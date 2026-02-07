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
    public class PageContentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PageContentsController> _logger;

        public PageContentsController(AppDbContext context, ILogger<PageContentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir tous les contenus de page
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PageContent>>> GetAll()
        {
            try
            {
                return await _context.PageContents
                    .OrderBy(p => p.NameFr)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des contenus de page");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir un contenu de page par son ID
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<PageContent>> GetById(int id)
        {
            try
            {
                var pageContent = await _context.PageContents.FindAsync(id);
                if (pageContent == null)
                {
                    return NotFound();
                }
                return pageContent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du contenu de page {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir un contenu de page par son code (accès public)
        /// </summary>
        [HttpGet("by-code/{code}")]
        [AllowAnonymous]
        public async Task<ActionResult<PageContent>> GetByCode(string code)
        {
            try
            {
                var pageContent = await _context.PageContents
                    .FirstOrDefaultAsync(p => p.Code == code);
                if (pageContent == null)
                {
                    return NotFound();
                }
                return pageContent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du contenu de page par code {Code}", code);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les contenus de page par structure pédagogique
        /// </summary>
        [HttpGet("pedagogicalstructure/{structureId}")]
        public async Task<ActionResult<IEnumerable<PageContent>>> GetByPedagogicalStructure(int structureId)
        {
            try
            {
                return await _context.PageContents
                    .Where(p => p.PedagogicalStructureId == structureId)
                    .OrderBy(p => p.NameFr)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des contenus de page pour la structure pédagogique {StructureId}", structureId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les pages de fin
        /// </summary>
        [HttpGet("end-pages")]
        public async Task<ActionResult<IEnumerable<PageContent>>> GetEndPages()
        {
            try
            {
                return await _context.PageContents
                    .Where(p => p.IsEndPage)
                    .OrderBy(p => p.NameFr)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des pages de fin");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les pages nécessitant un consentement
        /// </summary>
        [HttpGet("consent-pages")]
        public async Task<ActionResult<IEnumerable<PageContent>>> GetConsentPages()
        {
            try
            {
                return await _context.PageContents
                    .Where(p => p.RequiresConsent)
                    .OrderBy(p => p.NameFr)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des pages de consentement");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Créer un nouveau contenu de page
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PageContent>> Create(PageContent pageContent)
        {
            try
            {
                _context.PageContents.Add(pageContent);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = pageContent.Id }, pageContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création du contenu de page");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour un contenu de page
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PageContent pageContent)
        {
            if (id != pageContent.Id)
            {
                return BadRequest("L'ID ne correspond pas");
            }

            try
            {
                _context.Entry(pageContent).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await PageContentExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour du contenu de page {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer un contenu de page
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var pageContent = await _context.PageContents.FindAsync(id);
                if (pageContent == null)
                {
                    return NotFound();
                }

                _context.PageContents.Remove(pageContent);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression du contenu de page {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task<bool> PageContentExists(int id)
        {
            return await _context.PageContents.AnyAsync(e => e.Id == id);
        }
    }
}
