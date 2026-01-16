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
    public class PromptsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PromptsController> _logger;

        public PromptsController(AppDbContext context, ILogger<PromptsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir tous les prompts
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PromptDto>>> GetAll()
        {
            try
            {
                var prompts = await _context.Prompts.ToListAsync();
                return prompts.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des prompts");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir le nombre de prompts
        /// </summary>
        [HttpGet("count")]
        public async Task<ActionResult<int>> Count()
        {
            try
            {
                return await _context.Prompts.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du comptage des prompts");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir un prompt par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PromptDto>> GetById(int id)
        {
            try
            {
                var prompt = await _context.Prompts.FindAsync(id);
                if (prompt == null)
                {
                    return NotFound();
                }
                return MapToDto(prompt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du prompt {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir un prompt par son code
        /// </summary>
        [HttpGet("code/{code}")]
        public async Task<ActionResult<PromptDto>> GetByCode(string code)
        {
            try
            {
                var prompt = await _context.Prompts
                    .FirstOrDefaultAsync(p => p.Code == code);
                if (prompt == null)
                {
                    return NotFound();
                }
                return MapToDto(prompt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du prompt par code {Code}", code);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir toutes les versions d'un prompt
        /// </summary>
        [HttpGet("{id}/versions")]
        public async Task<ActionResult<IEnumerable<PromptVersionDto>>> GetVersions(int id)
        {
            try
            {
                var promptExists = await _context.Prompts.AnyAsync(p => p.Id == id);
                if (!promptExists)
                {
                    return NotFound();
                }

                var versions = await _context.PromptVersions
                    .Where(v => v.PromptId == id)
                    .OrderByDescending(v => v.CreatedAt)
                    .ToListAsync();

                return versions.Select(v => new PromptVersionDto
                {
                    Id = v.Id,
                    PromptId = v.PromptId,
                    Version = v.Version,
                    NewContent = v.NewContent,
                    Active = v.Active,
                    CreatedAt = v.CreatedAt,
                    UpdatedAt = v.UpdatedAt
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des versions du prompt {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir la version active d'un prompt
        /// </summary>
        [HttpGet("{id}/active-version")]
        public async Task<ActionResult<PromptVersionDto>> GetActiveVersion(int id)
        {
            try
            {
                var promptExists = await _context.Prompts.AnyAsync(p => p.Id == id);
                if (!promptExists)
                {
                    return NotFound("Prompt non trouvé");
                }

                var version = await _context.PromptVersions
                    .FirstOrDefaultAsync(v => v.PromptId == id && v.Active == true);

                if (version == null)
                {
                    return NotFound("Aucune version active");
                }

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de la version active du prompt {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Créer un nouveau prompt
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PromptDto>> Create(PromptCreateDto dto)
        {
            try
            {
                // Vérifier si le code existe déjà
                if (!string.IsNullOrEmpty(dto.Code) && await _context.Prompts.AnyAsync(p => p.Code == dto.Code))
                {
                    return BadRequest("Un prompt avec ce code existe déjà");
                }

                var prompt = new Prompt
                {
                    Code = dto.Code,
                    NameFr = dto.NameFr,
                    NameEn = dto.NameEn,
                    DescriptionFr = dto.DescriptionFr,
                    DescriptionEn = dto.DescriptionEn,
                    Content = dto.Content
                };

                _context.Prompts.Add(prompt);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = prompt.Id }, MapToDto(prompt));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création du prompt");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour un prompt
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PromptUpdateDto dto)
        {
            try
            {
                var prompt = await _context.Prompts.FindAsync(id);
                if (prompt == null)
                {
                    return NotFound();
                }

                // Vérifier si le code existe déjà pour un autre prompt
                if (!string.IsNullOrEmpty(dto.Code) && await _context.Prompts.AnyAsync(p => p.Code == dto.Code && p.Id != id))
                {
                    return BadRequest("Un autre prompt avec ce code existe déjà");
                }

                prompt.Code = dto.Code;
                prompt.NameFr = dto.NameFr;
                prompt.NameEn = dto.NameEn;
                prompt.DescriptionFr = dto.DescriptionFr;
                prompt.DescriptionEn = dto.DescriptionEn;
                prompt.Content = dto.Content;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour du prompt {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mise à jour partielle d'un prompt (PATCH)
        /// </summary>
        [HttpPatch("{id}")]
        public async Task<ActionResult<PromptDto>> Patch(int id, [FromBody] JObject updates)
        {
            try
            {
                if (updates == null)
                {
                    return BadRequest("Le corps de la requête doit être un objet JSON valide");
                }

                var prompt = await _context.Prompts.FindAsync(id);
                if (prompt == null)
                {
                    return NotFound();
                }

                // Vérifier si le code existe déjà pour un autre prompt
                if (updates.TryGetValue("code", out var codeToken) && codeToken.Type == JTokenType.String)
                {
                    var code = codeToken.Value<string>();
                    if (!string.IsNullOrEmpty(code) && await _context.Prompts.AnyAsync(p => p.Code == code && p.Id != id))
                    {
                        return BadRequest("Un autre prompt avec ce code existe déjà");
                    }
                    prompt.Code = code;
                }

                if (updates.TryGetValue("nameFr", out var nameFrToken))
                    prompt.NameFr = nameFrToken.Type == JTokenType.Null ? null : nameFrToken.Value<string>();

                if (updates.TryGetValue("nameEn", out var nameEnToken))
                    prompt.NameEn = nameEnToken.Type == JTokenType.Null ? null : nameEnToken.Value<string>();

                if (updates.TryGetValue("descriptionFr", out var descFrToken))
                    prompt.DescriptionFr = descFrToken.Type == JTokenType.Null ? null : descFrToken.Value<string>();

                if (updates.TryGetValue("descriptionEn", out var descEnToken))
                    prompt.DescriptionEn = descEnToken.Type == JTokenType.Null ? null : descEnToken.Value<string>();

                if (updates.TryGetValue("content", out var contentToken))
                    prompt.Content = contentToken.Type == JTokenType.Null ? null : contentToken.Value<string>();

                await _context.SaveChangesAsync();
                return MapToDto(prompt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour partielle du prompt {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer un prompt
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var prompt = await _context.Prompts.FindAsync(id);
                if (prompt == null)
                {
                    return NotFound();
                }

                // Vérifier s'il y a des versions
                var hasVersions = await _context.PromptVersions.AnyAsync(v => v.PromptId == id);
                if (hasVersions)
                {
                    return BadRequest("Impossible de supprimer un prompt ayant des versions");
                }

                _context.Prompts.Remove(prompt);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression du prompt {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Initialiser les prompts par défaut
        /// </summary>
        [HttpPost("initialize")]
        public async Task<ActionResult> Initialize()
        {
            try
            {
                var createdPrompts = new List<Prompt>();

                var defaultPrompts = new List<Prompt>
                {
                    new Prompt { Code = "SYSTEM_DEFAULT", NameFr = "Prompt système par défaut", NameEn = "Default system prompt", DescriptionFr = "Prompt système utilisé par défaut", DescriptionEn = "Default system prompt", Content = "You are a helpful assistant." },
                    new Prompt { Code = "ANALYSIS", NameFr = "Analyse de document", NameEn = "Document analysis", DescriptionFr = "Prompt pour l'analyse de documents", DescriptionEn = "Prompt for document analysis", Content = "Analyze the following document and provide insights." },
                    new Prompt { Code = "SUMMARY", NameFr = "Résumé", NameEn = "Summary", DescriptionFr = "Prompt pour générer des résumés", DescriptionEn = "Prompt for generating summaries", Content = "Summarize the following content concisely." }
                };

                foreach (var prompt in defaultPrompts)
                {
                    if (!await _context.Prompts.AnyAsync(p => p.Code == prompt.Code))
                    {
                        _context.Prompts.Add(prompt);
                        createdPrompts.Add(prompt);
                    }
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("{Count} prompts initialisés", createdPrompts.Count);
                return Ok(new { message = $"{createdPrompts.Count} prompt(s) créé(s)", prompts = createdPrompts.Select(MapToDto) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'initialisation des prompts");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private static PromptDto MapToDto(Prompt prompt)
        {
            return new PromptDto
            {
                Id = prompt.Id,
                Code = prompt.Code,
                NameFr = prompt.NameFr,
                NameEn = prompt.NameEn,
                DescriptionFr = prompt.DescriptionFr,
                DescriptionEn = prompt.DescriptionEn,
                Content = prompt.Content
            };
        }
    }
}
