using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using serveur.Data;
using serveur.Models.Dtos;
using serveur.Models.Entities;

namespace serveur.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TextAnnotationsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TextAnnotationsController> _logger;

        public TextAnnotationsController(AppDbContext context, ILogger<TextAnnotationsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir les annotations par contexte et apprenant
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TextAnnotationDto>>> GetByContext(
            [FromQuery] string contextId,
            [FromQuery] int learnerId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(contextId))
                {
                    return BadRequest("contextId est requis");
                }

                var annotations = await _context.TextAnnotations
                    .Where(a => a.ContextId == contextId && a.LearnerId == learnerId)
                    .OrderBy(a => a.CreatedAt)
                    .Select(a => new TextAnnotationDto
                    {
                        Id = a.Id,
                        LearnerId = a.LearnerId,
                        ContextId = a.ContextId,
                        AnnotationType = a.AnnotationType,
                        Color = a.Color,
                        TextContent = a.TextContent,
                        XPath = a.XPath,
                        StartOffset = a.StartOffset,
                        EndOffset = a.EndOffset,
                        CreatedAt = a.CreatedAt,
                        UpdatedAt = a.UpdatedAt
                    })
                    .ToListAsync();

                return annotations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la recuperation des annotations pour contextId={ContextId}, learnerId={LearnerId}", contextId, learnerId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir une annotation par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TextAnnotationDto>> GetById(int id)
        {
            try
            {
                var annotation = await _context.TextAnnotations
                    .Where(a => a.Id == id)
                    .Select(a => new TextAnnotationDto
                    {
                        Id = a.Id,
                        LearnerId = a.LearnerId,
                        ContextId = a.ContextId,
                        AnnotationType = a.AnnotationType,
                        Color = a.Color,
                        TextContent = a.TextContent,
                        XPath = a.XPath,
                        StartOffset = a.StartOffset,
                        EndOffset = a.EndOffset,
                        CreatedAt = a.CreatedAt,
                        UpdatedAt = a.UpdatedAt
                    })
                    .FirstOrDefaultAsync();

                if (annotation == null)
                {
                    return NotFound();
                }

                return annotation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la recuperation de l'annotation {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Creer une nouvelle annotation
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TextAnnotationDto>> Create(TextAnnotationCreateDto dto)
        {
            try
            {
                // Valider le type d'annotation
                var validTypes = new[] { "highlight", "underline", "circle", "strikethrough" };
                if (!validTypes.Contains(dto.AnnotationType))
                {
                    return BadRequest($"Type d'annotation invalide. Types valides: {string.Join(", ", validTypes)}");
                }

                var annotation = new TextAnnotation
                {
                    LearnerId = dto.LearnerId,
                    ContextId = dto.ContextId,
                    AnnotationType = dto.AnnotationType,
                    Color = dto.Color,
                    TextContent = dto.TextContent,
                    XPath = dto.XPath,
                    StartOffset = dto.StartOffset,
                    EndOffset = dto.EndOffset,
                    CreatedAt = DateTime.UtcNow
                };

                _context.TextAnnotations.Add(annotation);
                await _context.SaveChangesAsync();

                var result = new TextAnnotationDto
                {
                    Id = annotation.Id,
                    LearnerId = annotation.LearnerId,
                    ContextId = annotation.ContextId,
                    AnnotationType = annotation.AnnotationType,
                    Color = annotation.Color,
                    TextContent = annotation.TextContent,
                    XPath = annotation.XPath,
                    StartOffset = annotation.StartOffset,
                    EndOffset = annotation.EndOffset,
                    CreatedAt = annotation.CreatedAt,
                    UpdatedAt = annotation.UpdatedAt
                };

                return CreatedAtAction(nameof(GetById), new { id = annotation.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la creation de l'annotation");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre a jour une annotation (couleur et type seulement)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TextAnnotationUpdateDto dto)
        {
            try
            {
                var annotation = await _context.TextAnnotations.FindAsync(id);
                if (annotation == null)
                {
                    return NotFound();
                }

                // Valider le type d'annotation
                var validTypes = new[] { "highlight", "underline", "circle", "strikethrough" };
                if (!validTypes.Contains(dto.AnnotationType))
                {
                    return BadRequest($"Type d'annotation invalide. Types valides: {string.Join(", ", validTypes)}");
                }

                annotation.AnnotationType = dto.AnnotationType;
                annotation.Color = dto.Color;
                annotation.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise a jour de l'annotation {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer une annotation
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var annotation = await _context.TextAnnotations.FindAsync(id);
                if (annotation == null)
                {
                    return NotFound();
                }

                _context.TextAnnotations.Remove(annotation);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de l'annotation {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer toutes les annotations d'un contexte pour un apprenant
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> DeleteByContext(
            [FromQuery] string contextId,
            [FromQuery] int learnerId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(contextId))
                {
                    return BadRequest("contextId est requis");
                }

                var annotations = await _context.TextAnnotations
                    .Where(a => a.ContextId == contextId && a.LearnerId == learnerId)
                    .ToListAsync();

                if (annotations.Count == 0)
                {
                    return NoContent();
                }

                _context.TextAnnotations.RemoveRange(annotations);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression des annotations pour contextId={ContextId}, learnerId={LearnerId}", contextId, learnerId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }
    }
}
