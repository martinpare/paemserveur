using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using serveur.Data;
using serveur.Models.Entities;
using serveur.Models.Dtos;

namespace serveur.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UsersController> _logger;

        public UsersController(AppDbContext context, ILogger<UsersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir tous les utilisateurs (sans mot de passe)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                return users.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des utilisateurs");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir un utilisateur par son ID (sans mot de passe)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                return MapToDto(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'utilisateur {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir un utilisateur par son nom d'utilisateur
        /// </summary>
        [HttpGet("username/{username}")]
        public async Task<ActionResult<UserDto>> GetByUsername(string username)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == username);
                if (user == null)
                {
                    return NotFound();
                }
                return MapToDto(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'utilisateur {Username}", username);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les rôles d'un utilisateur pour une organisation
        /// </summary>
        [HttpGet("{userId}/roles")]
        public async Task<ActionResult<IEnumerable<Role>>> GetUserRoles(int userId, [FromQuery] int? organisationId = null)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return NotFound("Utilisateur non trouvé");
                }

                var query = _context.UserRoles
                    .Include(ur => ur.Role)
                    .Where(ur => ur.UserId == userId);

                if (organisationId.HasValue)
                {
                    query = query.Where(ur => ur.OrganisationId == organisationId.Value || ur.OrganisationId == null);
                }

                var roles = await query
                    .Select(ur => ur.Role)
                    .OrderBy(r => r.Level)
                    .ToListAsync();

                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des rôles de l'utilisateur {UserId}", userId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Synchroniser les rôles d'un utilisateur pour une organisation
        /// Supprime les anciens rôles et ajoute les nouveaux
        /// </summary>
        [HttpPut("{userId}/roles")]
        public async Task<IActionResult> SyncUserRoles(int userId, [FromBody] SyncUserRolesDto dto)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return NotFound("Utilisateur non trouvé");
                }

                // Récupérer les rôles actuels de l'utilisateur pour cette organisation
                var currentUserRoles = await _context.UserRoles
                    .Where(ur => ur.UserId == userId && ur.OrganisationId == dto.OrganisationId)
                    .ToListAsync();

                // Supprimer les rôles qui ne sont plus attribués
                var rolesToRemove = currentUserRoles
                    .Where(ur => !dto.RoleIds.Contains(ur.RoleId))
                    .ToList();
                _context.UserRoles.RemoveRange(rolesToRemove);

                // Ajouter les nouveaux rôles
                var currentRoleIds = currentUserRoles.Select(ur => ur.RoleId).ToList();
                var rolesToAdd = dto.RoleIds
                    .Where(roleId => !currentRoleIds.Contains(roleId))
                    .Select(roleId => new UserRole
                    {
                        UserId = userId,
                        RoleId = roleId,
                        OrganisationId = dto.OrganisationId
                    })
                    .ToList();
                _context.UserRoles.AddRange(rolesToAdd);

                await _context.SaveChangesAsync();

                _logger.LogInformation("Rôles synchronisés pour l'utilisateur {UserId} dans l'organisation {OrgId}: {RoleIds}",
                    userId, dto.OrganisationId, string.Join(", ", dto.RoleIds));

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la synchronisation des rôles de l'utilisateur {UserId}", userId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les utilisateurs d'une organisation
        /// </summary>
        [HttpGet("organisation/{organisationId}")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetByOrganisation(int organisationId)
        {
            try
            {
                var users = await _context.Users
                    .Where(u => u.OrganisationId == organisationId)
                    .ToListAsync();
                return users.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des utilisateurs de l'organisation {OrganisationId}", organisationId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Créer un nouvel utilisateur
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<UserDto>> Create(UserCreateDto userDto)
        {
            try
            {
                // Vérifier si le nom d'utilisateur existe déjà
                if (await _context.Users.AnyAsync(u => u.Username == userDto.Username))
                {
                    return BadRequest("Ce nom d'utilisateur existe déjà");
                }

                // Vérifier si l'email existe déjà
                if (await _context.Users.AnyAsync(u => u.Mail == userDto.Mail))
                {
                    return BadRequest("Cet email existe déjà");
                }

                var user = new User
                {
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    Sexe = userDto.Sexe,
                    Username = userDto.Username,
                    Mail = userDto.Mail,
                    Password = HashPassword(userDto.Password),
                    DarkMode = false,
                    OrganisationId = userDto.OrganisationId,
                    PedagogicalStructureId = userDto.PedagogicalStructureId,
                    LearningCenterId = userDto.LearningCenterId,
                    TitleId = userDto.TitleId
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = user.Id }, MapToDto(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de l'utilisateur");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour un utilisateur
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> Update(int id, UserUpdateDto userDto)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                // Vérifier si l'email existe déjà pour un autre utilisateur
                if (await _context.Users.AnyAsync(u => u.Mail == userDto.Mail && u.Id != id))
                {
                    return BadRequest("Cet email est déjà utilisé par un autre utilisateur");
                }

                user.FirstName = userDto.FirstName;
                user.LastName = userDto.LastName;
                user.Sexe = userDto.Sexe;
                user.Mail = userDto.Mail;
                user.OrganisationId = userDto.OrganisationId;
                user.PedagogicalStructureId = userDto.PedagogicalStructureId;
                user.LearningCenterId = userDto.LearningCenterId;
                user.TitleId = userDto.TitleId;

                await _context.SaveChangesAsync();
                return MapToDto(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de l'utilisateur {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mise à jour partielle d'un utilisateur (PATCH)
        /// Seuls les champs fournis sont mis à jour
        /// Supporte les valeurs null explicites (ex: avatar: null pour supprimer)
        /// </summary>
        [HttpPatch("{id}")]
        public async Task<ActionResult<UserDto>> Patch(int id, [FromBody] JObject updates)
        {
            try
            {
                if (updates == null)
                {
                    return BadRequest("Le corps de la requête doit être un objet JSON valide");
                }

                // Debug: Log les propriétés reçues
                var keys = updates.Properties().Select(p => p.Name).ToList();
                _logger.LogInformation("PATCH User {Id}: Clés reçues: {Keys}", id, string.Join(", ", keys));

                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                // Vérifier si l'email existe déjà pour un autre utilisateur
                if (updates.TryGetValue("mail", out var mailToken) && mailToken.Type == JTokenType.String)
                {
                    var mail = mailToken.Value<string>();
                    if (await _context.Users.AnyAsync(u => u.Mail == mail && u.Id != id))
                    {
                        return BadRequest("Cet email est déjà utilisé par un autre utilisateur");
                    }
                    user.Mail = mail;
                }

                // Mettre à jour uniquement les champs présents dans le JSON
                if (updates.TryGetValue("firstName", out var firstNameToken))
                    user.FirstName = firstNameToken.Type == JTokenType.Null ? null : firstNameToken.Value<string>();

                if (updates.TryGetValue("lastName", out var lastNameToken))
                    user.LastName = lastNameToken.Type == JTokenType.Null ? null : lastNameToken.Value<string>();

                if (updates.TryGetValue("sexe", out var sexeToken))
                    user.Sexe = sexeToken.Type == JTokenType.Null ? null : sexeToken.Value<string>();

                if (updates.TryGetValue("darkMode", out var darkModeToken) && darkModeToken.Type == JTokenType.Boolean)
                    user.DarkMode = darkModeToken.Value<bool>();

                if (updates.TryGetValue("avatar", out var avatarToken))
                    user.Avatar = avatarToken.Type == JTokenType.Null ? null : avatarToken.Value<string>();

                if (updates.TryGetValue("accentColor", out var accentColorToken))
                    user.AccentColor = accentColorToken.Type == JTokenType.Null ? null : accentColorToken.Value<string>();

                if (updates.TryGetValue("language", out var languageToken))
                    user.Language = languageToken.Type == JTokenType.Null ? null : languageToken.Value<string>();

                if (updates.TryGetValue("organisationId", out var orgToken))
                    user.OrganisationId = orgToken.Type == JTokenType.Null ? null : orgToken.Value<int>();

                if (updates.TryGetValue("pedagogicalStructureId", out var pedToken))
                    user.PedagogicalStructureId = pedToken.Type == JTokenType.Null ? null : pedToken.Value<int>();

                if (updates.TryGetValue("learningCenterId", out var lcToken))
                    user.LearningCenterId = lcToken.Type == JTokenType.Null ? null : lcToken.Value<int>();

                if (updates.TryGetValue("titleId", out var titleToken))
                    user.TitleId = titleToken.Type == JTokenType.Null ? null : titleToken.Value<int>();

                if (updates.TryGetValue("activeRoleId", out var activeRoleToken))
                    user.ActiveRoleId = activeRoleToken.Type == JTokenType.Null ? null : activeRoleToken.Value<int>();

                await _context.SaveChangesAsync();
                return MapToDto(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour partielle de l'utilisateur {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer l'avatar d'un utilisateur
        /// </summary>
        [HttpDelete("{id}/avatar")]
        public async Task<ActionResult<UserDto>> DeleteAvatar(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                user.Avatar = null;
                await _context.SaveChangesAsync();
                return MapToDto(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de l'avatar de l'utilisateur {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour le mot de passe d'un utilisateur
        /// </summary>
        [HttpPut("{id}/password")]
        public async Task<IActionResult> UpdatePassword(int id, PasswordChangeDto passwordDto)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                // Vérifier l'ancien mot de passe
                if (!VerifyPassword(passwordDto.CurrentPassword, user.Password))
                {
                    return BadRequest("Le mot de passe actuel est incorrect");
                }

                user.Password = HashPassword(passwordDto.NewPassword);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du changement de mot de passe de l'utilisateur {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour les préférences d'un utilisateur
        /// </summary>
        [HttpPut("{id}/preferences")]
        public async Task<IActionResult> UpdatePreferences(int id, UserPreferencesDto prefsDto)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                user.DarkMode = prefsDto.DarkMode;
                user.AccentColor = prefsDto.AccentColor;
                user.Language = prefsDto.Language;
                user.Avatar = prefsDto.Avatar;
                user.ActiveRoleId = prefsDto.ActiveRoleId;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour des préférences de l'utilisateur {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer un utilisateur
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de l'utilisateur {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Sexe = user.Sexe,
                Username = user.Username,
                Mail = user.Mail,
                DarkMode = user.DarkMode,
                Avatar = user.Avatar,
                AccentColor = user.AccentColor,
                Language = user.Language,
                OrganisationId = user.OrganisationId,
                PedagogicalStructureId = user.PedagogicalStructureId,
                LearningCenterId = user.LearningCenterId,
                TitleId = user.TitleId,
                ActiveRoleId = user.ActiveRoleId
            };
        }

        // Note: Dans un vrai projet, utilisez une bibliothèque comme BCrypt ou ASP.NET Core Identity
        private string HashPassword(string password)
        {
            // Implémentation simplifiée - À remplacer par une vraie fonction de hachage
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return HashPassword(password) == hashedPassword;
        }
    }
}
