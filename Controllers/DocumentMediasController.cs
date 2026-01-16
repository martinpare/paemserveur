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

namespace serveur.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DocumentMediasController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DocumentMediasController> _logger;

        public DocumentMediasController(AppDbContext context, ILogger<DocumentMediasController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir tous les médias
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DocumentMedia>>> GetAll()
        {
            try
            {
                return await _context.DocumentMedias.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des médias");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir un média par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<DocumentMedia>> GetById(int id)
        {
            try
            {
                var media = await _context.DocumentMedias.FindAsync(id);
                if (media == null)
                {
                    return NotFound();
                }
                return media;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du média {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les médias d'un document
        /// </summary>
        [HttpGet("document/{documentId}")]
        public async Task<ActionResult<IEnumerable<DocumentMedia>>> GetByDocument(int documentId)
        {
            try
            {
                return await _context.DocumentMedias
                    .Where(m => m.DocumentId == documentId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des médias du document {DocumentId}", documentId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Créer un nouveau média
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<DocumentMedia>> Create(DocumentMedia media)
        {
            try
            {
                media.UploadedAt = DateTime.UtcNow;
                _context.DocumentMedias.Add(media);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = media.Id }, media);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création du média");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour un média
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, DocumentMedia media)
        {
            if (id != media.Id)
            {
                return BadRequest("L'ID ne correspond pas");
            }

            try
            {
                _context.Entry(media).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await MediaExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour du média {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer un média
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var media = await _context.DocumentMedias.FindAsync(id);
                if (media == null)
                {
                    return NotFound();
                }

                _context.DocumentMedias.Remove(media);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression du média {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task<bool> MediaExists(int id)
        {
            return await _context.DocumentMedias.AnyAsync(e => e.Id == id);
        }
    }
}
