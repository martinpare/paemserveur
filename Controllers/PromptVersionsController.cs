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
    public class PromptVersionsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PromptVersionsController> _logger;

        public PromptVersionsController(AppDbContext context, ILogger<PromptVersionsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir les versions de prompts (avec filtres optionnels)
        /// ?promptId=X pour filtrer par prompt
        /// ?active=true pour ne récupérer que les versions actives
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PromptVersionDto>>> GetAll([FromQuery] int? promptId = null, [FromQuery] bool? active = null)
        {
            try
            {
                var query = _context.PromptVersions.AsQueryable();

                if (promptId.HasValue)
                {
                    query = query.Where(v => v.PromptId == promptId.Value);
                }

                if (active.HasValue)
                {
                    query = query.Where(v => v.Active == active.Value);
                }

                var versions = await query
                    .OrderByDescending(v => v.CreatedAt)
                    .ToListAsync();

                return versions.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des versions de prompts");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir le nombre de versions
        /// </summary>
        [HttpGet("count")]
        public async Task<ActionResult<int>> Count()
        {
            try
            {
                return await _context.PromptVersions.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du comptage des versions de prompts");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir une version par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PromptVersionDto>> GetById(int id)
        {
            try
            {
                var version = await _context.PromptVersions.FindAsync(id);
                if (version == null)
                {
                    return NotFound();
                }
                return MapToDto(version);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de la version {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Créer une nouvelle version de prompt
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PromptVersionDto>> Create(PromptVersionCreateDto dto)
        {
            try
            {
                // Vérifier que le prompt existe
                var promptExists = await _context.Prompts.AnyAsync(p => p.Id == dto.PromptId);
                if (!promptExists)
                {
                    return BadRequest("Le prompt spécifié n'existe pas");
                }

                // Vérifier si la version existe déjà pour ce prompt
                if (await _context.PromptVersions.AnyAsync(v => v.PromptId == dto.PromptId && v.Version == dto.Version))
                {
                    return BadRequest("Cette version existe déjà pour ce prompt");
                }

                // Si cette version est active, désactiver les autres
                if (dto.Active == true)
                {
                    var activeVersions = await _context.PromptVersions
                        .Where(v => v.PromptId == dto.PromptId && v.Active == true)
                        .ToListAsync();
                    foreach (var v in activeVersions)
                    {
                        v.Active = false;
                    }
                }

                var version = new PromptVersion
                {
                    PromptId = dto.PromptId,
                    Version = dto.Version,
                    NewContent = dto.NewContent,
                    Active = dto.Active,
                    CreatedAt = DateTime.UtcNow
                };

                _context.PromptVersions.Add(version);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = version.Id }, MapToDto(version));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de la version de prompt");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour une version de prompt
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PromptVersionUpdateDto dto)
        {
            try
            {
                var version = await _context.PromptVersions.FindAsync(id);
                if (version == null)
                {
                    return NotFound();
                }

                // Vérifier si la version existe déjà pour ce prompt (si modifiée)
                if (dto.Version != version.Version && await _context.PromptVersions.AnyAsync(v => v.PromptId == version.PromptId && v.Version == dto.Version && v.Id != id))
                {
                    return BadRequest("Cette version existe déjà pour ce prompt");
                }

                // Si cette version devient active, désactiver les autres
                if (dto.Active == true && version.Active != true)
                {
                    var activeVersions = await _context.PromptVersions
                        .Where(v => v.PromptId == version.PromptId && v.Active == true && v.Id != id)
                        .ToListAsync();
                    foreach (var v in activeVersions)
                    {
                        v.Active = false;
                    }
                }

                version.Version = dto.Version;
                version.NewContent = dto.NewContent;
                version.Active = dto.Active;
                version.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de la version {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mise à jour partielle d'une version (PATCH)
        /// </summary>
        [HttpPatch("{id}")]
        public async Task<ActionResult<PromptVersionDto>> Patch(int id, [FromBody] JObject updates)
        {
            try
            {
                if (updates == null)
                {
                    return BadRequest("Le corps de la requête doit être un objet JSON valide");
                }

                var version = await _context.PromptVersions.FindAsync(id);
                if (version == null)
                {
                    return NotFound();
                }

                if (updates.TryGetValue("version", out var versionToken) && versionToken.Type == JTokenType.String)
                {
                    var newVersion = versionToken.Value<string>();
                    if (newVersion != version.Version && await _context.PromptVersions.AnyAsync(v => v.PromptId == version.PromptId && v.Version == newVersion && v.Id != id))
                    {
                        return BadRequest("Cette version existe déjà pour ce prompt");
                    }
                    version.Version = newVersion;
                }

                if (updates.TryGetValue("newContent", out var contentToken))
                    version.NewContent = contentToken.Type == JTokenType.Null ? null : contentToken.Value<string>();

                if (updates.TryGetValue("active", out var activeToken) && activeToken.Type == JTokenType.Boolean)
                {
                    var newActive = activeToken.Value<bool>();
                    if (newActive && version.Active != true)
                    {
                        // Désactiver les autres versions du même prompt
                        var activeVersions = await _context.PromptVersions
                            .Where(v => v.PromptId == version.PromptId && v.Active == true && v.Id != id)
                            .ToListAsync();
                        foreach (var v in activeVersions)
                        {
                            v.Active = false;
                        }
                    }
                    version.Active = newActive;
                }

                version.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return MapToDto(version);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour partielle de la version {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer une version de prompt
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var version = await _context.PromptVersions.FindAsync(id);
                if (version == null)
                {
                    return NotFound();
                }

                // Vérifier s'il y a des commentaires
                var hasComments = await _context.PromptVersionComments.AnyAsync(c => c.PromptVersionId == id);
                if (hasComments)
                {
                    return BadRequest("Impossible de supprimer une version ayant des commentaires");
                }

                _context.PromptVersions.Remove(version);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de la version {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private static PromptVersionDto MapToDto(PromptVersion version)
        {
            return new PromptVersionDto
            {
                Id = version.Id,
                PromptId = version.PromptId,
                Version = version.Version,
                NewContent = version.NewContent,
                Active = version.Active,
                CreatedAt = version.CreatedAt,
                UpdatedAt = version.UpdatedAt
            };
        }
    }
}
