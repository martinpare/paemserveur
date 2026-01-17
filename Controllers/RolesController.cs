using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using serveur.Data;
using serveur.Models.Entities;
using serveur.Models.Dtos;

namespace serveur.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<RolesController> _logger;

        public RolesController(AppDbContext context, ILogger<RolesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir tous les rôles
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> GetAll()
        {
            try
            {
                return await _context.Roles
                    .OrderBy(r => r.Level)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des rôles");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir un rôle par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Role>> GetById(int id)
        {
            try
            {
                var role = await _context.Roles.FindAsync(id);
                if (role == null)
                {
                    return NotFound();
                }
                return role;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du rôle {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir un rôle par son code
        /// </summary>
        [HttpGet("code/{code}")]
        public async Task<ActionResult<Role>> GetByCode(string code)
        {
            try
            {
                var role = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Code == code);
                if (role == null)
                {
                    return NotFound();
                }
                return role;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du rôle par code {Code}", code);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les rôles d'une organisation
        /// </summary>
        [HttpGet("organisation/{organisationId}")]
        public async Task<ActionResult<IEnumerable<Role>>> GetByOrganisation(int organisationId)
        {
            try
            {
                return await _context.Roles
                    .Where(r => r.OrganisationId == organisationId || r.OrganisationId == null)
                    .OrderBy(r => r.Level)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des rôles de l'organisation {OrganisationId}", organisationId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les rôles actifs
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Role>>> GetActive()
        {
            try
            {
                return await _context.Roles
                    .Where(r => r.IsActive)
                    .OrderBy(r => r.Level)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des rôles actifs");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les fonctions effectives d'un rôle (avec héritage des rôles parents)
        /// Si le rôle a HasAllPermissions=true, retourne toutes les fonctions actives
        /// </summary>
        [HttpGet("{id}/functions")]
        public async Task<ActionResult<IEnumerable<string>>> GetRoleFunctions(int id)
        {
            try
            {
                var role = await _context.Roles.FindAsync(id);
                if (role == null)
                {
                    return NotFound("Rôle non trouvé");
                }

                // Si le rôle a toutes les permissions, retourner toutes les fonctions actives
                if (role.HasAllPermissions)
                {
                    var allFunctionCodes = await _context.Functions
                        .Where(f => f.IsActive)
                        .Select(f => f.Code)
                        .ToListAsync();
                    return Ok(allFunctionCodes);
                }

                // Collecter tous les IDs de rôles (incluant les parents)
                var roleIds = new List<int> { id };
                var currentRole = role;
                while (currentRole.ParentId.HasValue)
                {
                    roleIds.Add(currentRole.ParentId.Value);
                    currentRole = await _context.Roles.FindAsync(currentRole.ParentId.Value);
                    if (currentRole == null) break;

                    // Si un rôle parent a toutes les permissions, retourner toutes les fonctions
                    if (currentRole.HasAllPermissions)
                    {
                        var allFunctionCodes = await _context.Functions
                            .Where(f => f.IsActive)
                            .Select(f => f.Code)
                            .ToListAsync();
                        return Ok(allFunctionCodes);
                    }
                }

                // Récupérer tous les codes de fonctions assignés à ces rôles
                var functionCodes = await _context.RoleFunctions
                    .Where(rf => roleIds.Contains(rf.RoleId))
                    .Select(rf => rf.FunctionCode)
                    .Distinct()
                    .ToListAsync();

                return Ok(functionCodes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des fonctions du rôle {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour les fonctions d'un rôle (sync)
        /// </summary>
        [HttpPut("{id}/functions")]
        public async Task<IActionResult> UpdateRoleFunctions(int id, [FromBody] UpdateRoleFunctionsDto dto)
        {
            try
            {
                var role = await _context.Roles.FindAsync(id);
                if (role == null)
                {
                    return NotFound("Rôle non trouvé");
                }

                // Supprimer les anciennes assignations
                var existingAssignments = await _context.RoleFunctions
                    .Where(rf => rf.RoleId == id)
                    .ToListAsync();
                _context.RoleFunctions.RemoveRange(existingAssignments);

                // Créer les nouvelles assignations
                if (dto.FunctionIds != null && dto.FunctionIds.Any())
                {
                    var newAssignments = dto.FunctionIds.Select(code => new RoleFunction
                    {
                        RoleId = id,
                        FunctionCode = code
                    });
                    _context.RoleFunctions.AddRange(newAssignments);
                }

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour des fonctions du rôle {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les enfants d'un rôle
        /// </summary>
        [HttpGet("{id}/children")]
        public async Task<ActionResult<IEnumerable<Role>>> GetChildren(int id)
        {
            try
            {
                return await _context.Roles
                    .Where(r => r.ParentId == id)
                    .OrderBy(r => r.Level)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des enfants du rôle {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les rôles en structure arborescente
        /// </summary>
        [HttpGet("tree")]
        public async Task<ActionResult<IEnumerable<RoleTreeDto>>> GetTree()
        {
            try
            {
                var allRoles = await _context.Roles
                    .OrderBy(r => r.Level)
                    .ToListAsync();

                var roots = allRoles.Where(r => r.ParentId == null);
                return Ok(BuildRoleTree(roots, allRoles));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'arbre des rôles");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private List<RoleTreeDto> BuildRoleTree(IEnumerable<Role> nodes, List<Role> allRoles)
        {
            return nodes.Select(r => new RoleTreeDto
            {
                Id = r.Id,
                Code = r.Code,
                NameFr = r.NameFr,
                NameEn = r.NameEn,
                DescriptionFr = r.DescriptionFr,
                DescriptionEn = r.DescriptionEn,
                OrganisationId = r.OrganisationId,
                Level = r.Level,
                IsSystem = r.IsSystem,
                IsActive = r.IsActive,
                HasAllPermissions = r.HasAllPermissions,
                Children = BuildRoleTree(allRoles.Where(c => c.ParentId == r.Id), allRoles)
            }).ToList();
        }

        /// <summary>
        /// Créer un nouveau rôle
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Role>> Create([FromBody] Role role)
        {
            try
            {
                // Vérifier si le code existe déjà dans la même organisation
                if (await _context.Roles.AnyAsync(r => r.Code.ToLower() == role.Code.ToLower()
                    && r.OrganisationId == role.OrganisationId))
                {
                    return BadRequest("Un rôle avec ce code existe déjà dans cette organisation");
                }

                _context.Roles.Add(role);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = role.Id }, role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création du rôle");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour un rôle
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Role role)
        {
            _logger.LogInformation("Update role: id={Id}, role.Id={RoleId}, role.Code={Code}", id, role?.Id, role?.Code);

            // Vérifier si le modèle est valide
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _logger.LogWarning("ModelState invalid: {Errors}", string.Join(", ", errors));
                return BadRequest(ModelState);
            }

            if (role == null)
            {
                return BadRequest("Le corps de la requête est vide");
            }

            // Assigner l'ID depuis l'URL pour éviter les problèmes de binding
            role.Id = id;

            try
            {
                // Vérifier si le code existe déjà pour un autre rôle dans la même organisation
                var duplicateRole = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Code.ToLower() == role.Code.ToLower()
                        && r.Id != id
                        && r.OrganisationId == role.OrganisationId);
                if (duplicateRole != null)
                {
                    _logger.LogWarning("Duplicate code found: role.Id={RoleId}, duplicateRole.Id={DuplicateId}, code={Code}",
                        id, duplicateRole.Id, role.Code);
                    return BadRequest($"Un autre rôle (ID: {duplicateRole.Id}) avec ce code existe déjà dans cette organisation");
                }

                _context.Entry(role).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await RoleExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour du rôle {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer un rôle
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Vérifier si c'est un rôle système
                var role = await _context.Roles.FindAsync(id);
                if (role == null)
                {
                    return NotFound();
                }

                if (role.IsSystem)
                {
                    return BadRequest("Impossible de supprimer un rôle système");
                }

                // Vérifier s'il y a des enfants
                var hasChildren = await _context.Roles.AnyAsync(r => r.ParentId == id);
                if (hasChildren)
                {
                    return BadRequest("Impossible de supprimer un rôle ayant des enfants");
                }

                // Vérifier s'il y a des utilisateurs avec ce rôle
                var hasUsers = await _context.UserRoles.AnyAsync(ur => ur.RoleId == id);
                if (hasUsers)
                {
                    return BadRequest("Impossible de supprimer un rôle assigné à des utilisateurs");
                }

                // Supprimer les fonctions associées au rôle
                var roleFunctions = await _context.RoleFunctions.Where(rf => rf.RoleId == id).ToListAsync();
                _context.RoleFunctions.RemoveRange(roleFunctions);

                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression du rôle {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Initialiser les rôles système par défaut
        /// </summary>
        [HttpPost("initialize")]
        public async Task<ActionResult<IEnumerable<Role>>> Initialize()
        {
            try
            {
                var createdRoles = new List<Role>();

                var defaultRoles = new List<Role>
                {
                    new Role { Code = "SUPER_ADMIN", NameFr = "Super Administrateur", NameEn = "Super Administrator", DescriptionFr = "Accès complet au système", DescriptionEn = "Full system access", Level = 0, IsSystem = true, IsActive = true, HasAllPermissions = true },
                    new Role { Code = "ADMIN", NameFr = "Administrateur", NameEn = "Administrator", DescriptionFr = "Administration du système", DescriptionEn = "System administration", Level = 1, IsSystem = true, IsActive = true, HasAllPermissions = false },
                    new Role { Code = "MANAGER", NameFr = "Gestionnaire", NameEn = "Manager", DescriptionFr = "Gestion des ressources", DescriptionEn = "Resource management", Level = 2, IsSystem = true, IsActive = true, HasAllPermissions = false },
                    new Role { Code = "USER", NameFr = "Utilisateur", NameEn = "User", DescriptionFr = "Utilisateur standard", DescriptionEn = "Standard user", Level = 3, IsSystem = true, IsActive = true, HasAllPermissions = false },
                    new Role { Code = "GUEST", NameFr = "Invité", NameEn = "Guest", DescriptionFr = "Accès limité en lecture", DescriptionEn = "Limited read access", Level = 4, IsSystem = true, IsActive = true, HasAllPermissions = false }
                };

                foreach (var role in defaultRoles)
                {
                    if (!await _context.Roles.AnyAsync(r => r.Code == role.Code))
                    {
                        _context.Roles.Add(role);
                        createdRoles.Add(role);
                    }
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("{Count} rôles initialisés", createdRoles.Count);
                return Ok(new { message = $"{createdRoles.Count} rôle(s) créé(s)", roles = createdRoles });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'initialisation des rôles");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task<bool> RoleExists(int id)
        {
            return await _context.Roles.AnyAsync(e => e.Id == id);
        }
    }
}
