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
    public class DocumentElementsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DocumentElementsController> _logger;

        public DocumentElementsController(AppDbContext context, ILogger<DocumentElementsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir tous les éléments de documents
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DocumentElement>>> GetAll()
        {
            try
            {
                return await _context.DocumentElements
                    .OrderBy(e => e.SortOrder)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des éléments");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir un élément par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<DocumentElement>> GetById(int id)
        {
            try
            {
                var element = await _context.DocumentElements.FindAsync(id);
                if (element == null)
                {
                    return NotFound();
                }
                return element;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'élément {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les éléments d'un document
        /// </summary>
        [HttpGet("document/{documentId}")]
        public async Task<ActionResult<IEnumerable<DocumentElement>>> GetByDocument(int documentId)
        {
            try
            {
                return await _context.DocumentElements
                    .Where(e => e.DocumentId == documentId)
                    .OrderBy(e => e.SortOrder)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des éléments du document {DocumentId}", documentId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Créer un nouvel élément
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<DocumentElement>> Create(DocumentElement element)
        {
            try
            {
                // Calculer l'ordre si non spécifié
                if (!element.SortOrder.HasValue)
                {
                    var maxOrder = await _context.DocumentElements
                        .Where(e => e.DocumentId == element.DocumentId)
                        .MaxAsync(e => (int?)e.SortOrder) ?? 0;
                    element.SortOrder = maxOrder + 1;
                }

                _context.DocumentElements.Add(element);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = element.Id }, element);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de l'élément");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Créer plusieurs éléments en lot
        /// </summary>
        [HttpPost("bulk")]
        public async Task<ActionResult<IEnumerable<DocumentElement>>> CreateBulk([FromBody] List<DocumentElement> elements)
        {
            try
            {
                _context.DocumentElements.AddRange(elements);
                await _context.SaveChangesAsync();
                return Ok(elements);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création en lot des éléments");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour un élément
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, DocumentElement element)
        {
            if (id != element.Id)
            {
                return BadRequest("L'ID ne correspond pas");
            }

            try
            {
                _context.Entry(element).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ElementExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de l'élément {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Réorganiser les éléments d'un document
        /// </summary>
        [HttpPut("document/{documentId}/reorder")]
        public async Task<IActionResult> Reorder(int documentId, [FromBody] List<int> elementIds)
        {
            try
            {
                var elements = await _context.DocumentElements
                    .Where(e => e.DocumentId == documentId)
                    .ToListAsync();

                for (int i = 0; i < elementIds.Count; i++)
                {
                    var element = elements.FirstOrDefault(e => e.Id == elementIds[i]);
                    if (element != null)
                    {
                        element.SortOrder = i + 1;
                    }
                }

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la réorganisation des éléments du document {DocumentId}", documentId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer un élément
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var element = await _context.DocumentElements.FindAsync(id);
                if (element == null)
                {
                    return NotFound();
                }

                _context.DocumentElements.Remove(element);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de l'élément {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task<bool> ElementExists(int id)
        {
            return await _context.DocumentElements.AnyAsync(e => e.Id == id);
        }
    }
}
