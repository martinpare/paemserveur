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
    public class TemplatePagesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TemplatePagesController> _logger;

        public TemplatePagesController(AppDbContext context, ILogger<TemplatePagesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir toutes les pages de templates
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TemplatePage>>> GetAll()
        {
            try
            {
                return await _context.TemplatePages
                    .OrderBy(p => p.SortOrder)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des pages de templates");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir une page par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TemplatePage>> GetById(int id)
        {
            try
            {
                var page = await _context.TemplatePages.FindAsync(id);
                if (page == null)
                {
                    return NotFound();
                }
                return page;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de la page {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les pages d'un template
        /// </summary>
        [HttpGet("document/{documentId}")]
        public async Task<ActionResult<IEnumerable<TemplatePage>>> GetByDocument(int documentId)
        {
            try
            {
                return await _context.TemplatePages
                    .Where(p => p.TemplateDocumentId == documentId)
                    .OrderBy(p => p.SortOrder)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des pages du template {DocumentId}", documentId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Créer une nouvelle page
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TemplatePage>> Create(TemplatePage page)
        {
            try
            {
                _context.TemplatePages.Add(page);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = page.Id }, page);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de la page");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour une page
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TemplatePage page)
        {
            if (id != page.Id)
            {
                return BadRequest("L'ID ne correspond pas");
            }

            try
            {
                _context.Entry(page).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await PageExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de la page {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Réorganiser les pages d'un template
        /// </summary>
        [HttpPut("document/{documentId}/reorder")]
        public async Task<IActionResult> Reorder(int documentId, [FromBody] List<int> pageIds)
        {
            try
            {
                var pages = await _context.TemplatePages
                    .Where(p => p.TemplateDocumentId == documentId)
                    .ToListAsync();

                for (int i = 0; i < pageIds.Count; i++)
                {
                    var page = pages.FirstOrDefault(p => p.Id == pageIds[i]);
                    if (page != null)
                    {
                        page.SortOrder = i + 1;
                    }
                }

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la réorganisation des pages du template {DocumentId}", documentId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer une page
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var page = await _context.TemplatePages.FindAsync(id);
                if (page == null)
                {
                    return NotFound();
                }

                _context.TemplatePages.Remove(page);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de la page {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task<bool> PageExists(int id)
        {
            return await _context.TemplatePages.AnyAsync(e => e.Id == id);
        }
    }
}
