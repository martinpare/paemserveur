using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
    public class PromptVersionCommentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PromptVersionCommentsController> _logger;

        public PromptVersionCommentsController(AppDbContext context, ILogger<PromptVersionCommentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir les commentaires (avec filtre optionnel)
        /// ?promptVersionId=X pour filtrer par version
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PromptVersionCommentDto>>> GetAll([FromQuery] int? promptVersionId = null)
        {
            try
            {
                var query = _context.PromptVersionComments
                    .Include(c => c.User)
                    .AsQueryable();

                if (promptVersionId.HasValue)
                {
                    query = query.Where(c => c.PromptVersionId == promptVersionId.Value);
                }

                var comments = await query
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

                return comments.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des commentaires");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir le nombre de commentaires
        /// </summary>
        [HttpGet("count")]
        public async Task<ActionResult<int>> Count()
        {
            try
            {
                return await _context.PromptVersionComments.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du comptage des commentaires");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir un commentaire par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PromptVersionCommentDto>> GetById(int id)
        {
            try
            {
                var comment = await _context.PromptVersionComments
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.Id == id);
                if (comment == null)
                {
                    return NotFound();
                }
                return MapToDto(comment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du commentaire {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Créer un nouveau commentaire
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PromptVersionCommentDto>> Create(PromptVersionCommentCreateDto dto)
        {
            try
            {
                // Vérifier que la version existe
                var versionExists = await _context.PromptVersions.AnyAsync(v => v.Id == dto.PromptVersionId);
                if (!versionExists)
                {
                    return BadRequest("La version spécifiée n'existe pas");
                }

                // Récupérer l'ID de l'utilisateur connecté depuis le token JWT
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized("Utilisateur non identifié");
                }

                var comment = new PromptVersionComment
                {
                    PromptVersionId = dto.PromptVersionId,
                    UserId = userId,
                    Content = dto.Content,
                    CreatedAt = DateTime.UtcNow
                };

                _context.PromptVersionComments.Add(comment);
                await _context.SaveChangesAsync();

                // Recharger avec l'utilisateur pour le DTO
                await _context.Entry(comment).Reference(c => c.User).LoadAsync();

                return CreatedAtAction(nameof(GetById), new { id = comment.Id }, MapToDto(comment));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création du commentaire");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour un commentaire
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PromptVersionCommentUpdateDto dto)
        {
            try
            {
                var comment = await _context.PromptVersionComments.FindAsync(id);
                if (comment == null)
                {
                    return NotFound();
                }

                // Vérifier que l'utilisateur est bien l'auteur du commentaire
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized("Utilisateur non identifié");
                }

                if (comment.UserId != userId)
                {
                    return Forbid("Vous ne pouvez modifier que vos propres commentaires");
                }

                comment.Content = dto.Content;
                comment.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour du commentaire {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer un commentaire
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var comment = await _context.PromptVersionComments.FindAsync(id);
                if (comment == null)
                {
                    return NotFound();
                }

                // Vérifier que l'utilisateur est bien l'auteur du commentaire
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized("Utilisateur non identifié");
                }

                if (comment.UserId != userId)
                {
                    return Forbid("Vous ne pouvez supprimer que vos propres commentaires");
                }

                _context.PromptVersionComments.Remove(comment);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression du commentaire {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private static PromptVersionCommentDto MapToDto(PromptVersionComment comment)
        {
            return new PromptVersionCommentDto
            {
                Id = comment.Id,
                PromptVersionId = comment.PromptVersionId,
                UserId = comment.UserId,
                UserName = comment.User != null ? $"{comment.User.FirstName} {comment.User.LastName}" : null,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt
            };
        }
    }
}
