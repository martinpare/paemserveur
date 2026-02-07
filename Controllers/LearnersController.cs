using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using serveur.Data;
using serveur.Models.Entities;

namespace serveur.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LearnersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<LearnersController> _logger;

        public LearnersController(AppDbContext context, ILogger<LearnersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir tous les apprenants
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Learner>>> GetAll()
        {
            try
            {
                return await _context.Learners
                    .Include(l => l.LearningCenter)
                    .Include(l => l.Group)
                    .Include(l => l.Language)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la recuperation des apprenants");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir un apprenant par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Learner>> GetById(int id)
        {
            try
            {
                var learner = await _context.Learners
                    .Include(l => l.LearningCenter)
                    .Include(l => l.Group)
                    .Include(l => l.Language)
                    .FirstOrDefaultAsync(l => l.Id == id);

                if (learner == null)
                {
                    return NotFound();
                }
                return learner;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la recuperation de l'apprenant {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les apprenants par centre d'apprentissage
        /// </summary>
        [HttpGet("by-learning-center/{learningCenterId}")]
        public async Task<ActionResult<IEnumerable<Learner>>> GetByLearningCenter(int learningCenterId)
        {
            try
            {
                return await _context.Learners
                    .Where(l => l.LearningCenterId == learningCenterId)
                    .Include(l => l.Group)
                    .Include(l => l.Language)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la recuperation des apprenants du centre {LearningCenterId}", learningCenterId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les apprenants par groupe
        /// </summary>
        [HttpGet("by-group/{groupId}")]
        public async Task<ActionResult<IEnumerable<Learner>>> GetByGroup(int groupId)
        {
            try
            {
                return await _context.Learners
                    .Where(l => l.GroupId == groupId)
                    .Include(l => l.LearningCenter)
                    .Include(l => l.Language)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la recuperation des apprenants du groupe {GroupId}", groupId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir un apprenant par son code permanent
        /// </summary>
        [HttpGet("by-permanent-code/{permanentCode}")]
        public async Task<ActionResult<Learner>> GetByPermanentCode(string permanentCode)
        {
            try
            {
                var learner = await _context.Learners
                    .Include(l => l.LearningCenter)
                    .Include(l => l.Group)
                    .Include(l => l.Language)
                    .FirstOrDefaultAsync(l => l.PermanentCode == permanentCode);

                if (learner == null)
                {
                    return NotFound();
                }
                return learner;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la recuperation de l'apprenant par code permanent {PermanentCode}", permanentCode);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir plusieurs apprenants par leurs codes permanents (pour connexion rapide)
        /// En mode développement, retourne également le mot de passe pour faciliter la connexion rapide
        /// </summary>
        [HttpPost("by-permanent-codes")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<object>>> GetByPermanentCodes(
            [FromBody] PermanentCodesRequest request,
            [FromServices] IWebHostEnvironment env)
        {
            try
            {
                if (request?.PermanentCodes == null || request.PermanentCodes.Count == 0)
                {
                    return Ok(new List<object>());
                }

                var isDevelopment = env.IsDevelopment();

                if (isDevelopment)
                {
                    // En mode développement, inclure le mot de passe pour la connexion rapide
                    var learnersWithPassword = await _context.Learners
                        .Where(l => request.PermanentCodes.Contains(l.PermanentCode) && l.IsActive)
                        .Select(l => new
                        {
                            l.Id,
                            l.PermanentCode,
                            l.FirstName,
                            l.LastName,
                            l.Avatar,
                            l.NativePassword
                        })
                        .ToListAsync();

                    return Ok(learnersWithPassword);
                }
                else
                {
                    // En production, ne pas inclure le mot de passe
                    var learners = await _context.Learners
                        .Where(l => request.PermanentCodes.Contains(l.PermanentCode) && l.IsActive)
                        .Select(l => new
                        {
                            l.Id,
                            l.PermanentCode,
                            l.FirstName,
                            l.LastName,
                            l.Avatar
                        })
                        .ToListAsync();

                    return Ok(learners);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la recuperation des apprenants par codes permanents");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        public class PermanentCodesRequest
        {
            public List<string> PermanentCodes { get; set; } = new();
        }

        /// <summary>
        /// Creer un nouvel apprenant
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Learner>> Create(Learner learner)
        {
            try
            {
                // Valider l'unicite du code permanent
                if (!string.IsNullOrWhiteSpace(learner.PermanentCode))
                {
                    var existingByCode = await _context.Learners
                        .AnyAsync(l => l.PermanentCode == learner.PermanentCode);
                    if (existingByCode)
                    {
                        return Conflict(new
                        {
                            error = "duplicate_permanent_code",
                            field = "permanentCode",
                            value = learner.PermanentCode,
                            message = $"Le code permanent '{learner.PermanentCode}' existe deja"
                        });
                    }
                }

                // Valider l'unicite du courriel
                if (!string.IsNullOrWhiteSpace(learner.Email))
                {
                    var existingByEmail = await _context.Learners
                        .AnyAsync(l => l.Email == learner.Email);
                    if (existingByEmail)
                    {
                        return Conflict(new
                        {
                            error = "duplicate_email",
                            field = "email",
                            value = learner.Email,
                            message = $"Le courriel '{learner.Email}' existe deja"
                        });
                    }
                }

                learner.CreatedAt = DateTime.UtcNow;
                learner.ModifiedAt = DateTime.UtcNow;

                // Hacher le mot de passe si fourni
                if (!string.IsNullOrWhiteSpace(learner.NativePassword))
                {
                    learner.NativePassword = BCrypt.Net.BCrypt.HashPassword(learner.NativePassword);
                }

                _context.Learners.Add(learner);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = learner.Id }, learner);
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message?.Contains("UNIQUE") == true ||
                                                ex.InnerException?.Message?.Contains("duplicate") == true)
            {
                _logger.LogWarning(ex, "Tentative de creation d'un apprenant avec des donnees en doublon");
                return Conflict(new
                {
                    error = "duplicate_entry",
                    message = "Un apprenant avec ces informations existe deja"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la creation de l'apprenant");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre a jour un apprenant
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Learner learner)
        {
            if (id != learner.Id)
            {
                return BadRequest("L'ID ne correspond pas");
            }

            try
            {
                // Valider l'unicite du code permanent (exclure l'apprenant actuel)
                if (!string.IsNullOrWhiteSpace(learner.PermanentCode))
                {
                    var existingByCode = await _context.Learners
                        .AnyAsync(l => l.PermanentCode == learner.PermanentCode && l.Id != id);
                    if (existingByCode)
                    {
                        return Conflict(new
                        {
                            error = "duplicate_permanent_code",
                            field = "permanentCode",
                            value = learner.PermanentCode,
                            message = $"Le code permanent '{learner.PermanentCode}' existe deja"
                        });
                    }
                }

                // Valider l'unicite du courriel (exclure l'apprenant actuel)
                if (!string.IsNullOrWhiteSpace(learner.Email))
                {
                    var existingByEmail = await _context.Learners
                        .AnyAsync(l => l.Email == learner.Email && l.Id != id);
                    if (existingByEmail)
                    {
                        return Conflict(new
                        {
                            error = "duplicate_email",
                            field = "email",
                            value = learner.Email,
                            message = $"Le courriel '{learner.Email}' existe deja"
                        });
                    }
                }

                // Hacher le mot de passe si fourni et non déjà haché (BCrypt commence par $2)
                if (!string.IsNullOrWhiteSpace(learner.NativePassword) && !learner.NativePassword.StartsWith("$2"))
                {
                    learner.NativePassword = BCrypt.Net.BCrypt.HashPassword(learner.NativePassword);
                }

                // Si hasAccommodations passe à false, supprimer les outils technologiques associés
                if (!learner.HasAccommodations)
                {
                    var toolsToRemove = await _context.LearnerTechnologicalTools
                        .Where(ltt => ltt.LearnerId == id)
                        .ToListAsync();

                    if (toolsToRemove.Any())
                    {
                        _context.LearnerTechnologicalTools.RemoveRange(toolsToRemove);
                    }
                }

                learner.ModifiedAt = DateTime.UtcNow;
                _context.Entry(learner).State = EntityState.Modified;
                _context.Entry(learner).Property(x => x.CreatedAt).IsModified = false;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await LearnerExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message?.Contains("UNIQUE") == true ||
                                                ex.InnerException?.Message?.Contains("duplicate") == true)
            {
                _logger.LogWarning(ex, "Tentative de mise a jour d'un apprenant avec des donnees en doublon");
                return Conflict(new
                {
                    error = "duplicate_entry",
                    message = "Un apprenant avec ces informations existe deja"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise a jour de l'apprenant {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mise à jour partielle d'un apprenant (PATCH)
        /// Seuls les champs fournis sont mis à jour
        /// Supporte les valeurs null explicites (ex: avatar: null pour supprimer)
        /// </summary>
        [HttpPatch("{id}")]
        public async Task<ActionResult<Learner>> Patch(int id, [FromBody] JObject updates)
        {
            try
            {
                if (updates == null)
                {
                    return BadRequest("Le corps de la requête doit être un objet JSON valide");
                }

                var learner = await _context.Learners.FindAsync(id);
                if (learner == null)
                {
                    return NotFound();
                }

                // Mettre à jour uniquement les champs présents dans le JSON
                if (updates.TryGetValue("avatar", out var avatarToken))
                    learner.Avatar = avatarToken.Type == JTokenType.Null ? null : avatarToken.Value<string>();

                if (updates.TryGetValue("firstName", out var firstNameToken) && firstNameToken.Type == JTokenType.String)
                    learner.FirstName = firstNameToken.Value<string>();

                if (updates.TryGetValue("lastName", out var lastNameToken) && lastNameToken.Type == JTokenType.String)
                    learner.LastName = lastNameToken.Value<string>();

                if (updates.TryGetValue("email", out var emailToken))
                    learner.Email = emailToken.Type == JTokenType.Null ? null : emailToken.Value<string>();

                if (updates.TryGetValue("permanentCode", out var codeToken))
                    learner.PermanentCode = codeToken.Type == JTokenType.Null ? null : codeToken.Value<string>();

                if (updates.TryGetValue("hasAccommodations", out var hasAccToken) && hasAccToken.Type == JTokenType.Boolean)
                {
                    learner.HasAccommodations = hasAccToken.Value<bool>();

                    // Si hasAccommodations passe à false, supprimer les outils technologiques associés
                    if (!learner.HasAccommodations)
                    {
                        var toolsToRemove = await _context.LearnerTechnologicalTools
                            .Where(ltt => ltt.LearnerId == id)
                            .ToListAsync();

                        if (toolsToRemove.Any())
                        {
                            _context.LearnerTechnologicalTools.RemoveRange(toolsToRemove);
                        }
                    }
                }

                if (updates.TryGetValue("isActive", out var activeToken) && activeToken.Type == JTokenType.Boolean)
                    learner.IsActive = activeToken.Value<bool>();

                if (updates.TryGetValue("groupId", out var groupToken))
                    learner.GroupId = groupToken.Type == JTokenType.Null ? null : groupToken.Value<int>();

                if (updates.TryGetValue("languageId", out var langToken) && langToken.Type == JTokenType.Integer)
                    learner.LanguageId = langToken.Value<int>();

                learner.ModifiedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return learner;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour partielle de l'apprenant {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer un apprenant
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var learner = await _context.Learners.FindAsync(id);
                if (learner == null)
                {
                    return NotFound();
                }

                _context.Learners.Remove(learner);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de l'apprenant {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task<bool> LearnerExists(int id)
        {
            return await _context.Learners.AnyAsync(e => e.Id == id);
        }

        /// <summary>
        /// Obtenir les outils technologiques associés à un apprenant
        /// </summary>
        [HttpGet("{id}/technological-tools")]
        public async Task<ActionResult<IEnumerable<object>>> GetTechnologicalTools(int id)
        {
            try
            {
                if (!await LearnerExists(id))
                {
                    return NotFound();
                }

                var tools = await _context.LearnerTechnologicalTools
                    .Where(ltt => ltt.LearnerId == id)
                    .Include(ltt => ltt.TechnologicalTool)
                    .Select(ltt => new
                    {
                        ltt.Id,
                        ltt.TechnologicalToolId,
                        ltt.Data,
                        ltt.CreatedAt,
                        TechnologicalTool = new
                        {
                            ltt.TechnologicalTool.Id,
                            ltt.TechnologicalTool.Code,
                            ltt.TechnologicalTool.NameFr,
                            ltt.TechnologicalTool.NameEn,
                            ltt.TechnologicalTool.DescriptionFr,
                            ltt.TechnologicalTool.DescriptionEn,
                            ltt.TechnologicalTool.Icon,
                            ltt.TechnologicalTool.DisplayOrder,
                            ltt.TechnologicalTool.IsActive,
                            ltt.TechnologicalTool.AdaptiveMeasure,
                            ltt.TechnologicalTool.DefaultData,
                            ltt.TechnologicalTool.Interface
                        }
                    })
                    .OrderBy(t => t.TechnologicalTool.DisplayOrder)
                    .ToListAsync();

                return Ok(tools);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des outils technologiques de l'apprenant {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }
    }
}
