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
    public class AnalysesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AnalysesController> _logger;

        public AnalysesController(AppDbContext context, ILogger<AnalysesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir toutes les analyses
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Analysis>>> GetAll()
        {
            try
            {
                return await _context.Analyses
                    .OrderByDescending(a => a.Date)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des analyses");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir une analyse par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Analysis>> GetById(int id)
        {
            try
            {
                var analysis = await _context.Analyses.FindAsync(id);
                if (analysis == null)
                {
                    return NotFound();
                }
                return analysis;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'analyse {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les analyses d'un item
        /// </summary>
        [HttpGet("item/{itemId}")]
        public async Task<ActionResult<IEnumerable<Analysis>>> GetByItem(int itemId)
        {
            try
            {
                return await _context.Analyses
                    .Where(a => a.ItemId == itemId)
                    .OrderByDescending(a => a.Date)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des analyses de l'item {ItemId}", itemId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Créer une nouvelle analyse
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Analysis>> Create(Analysis analysis)
        {
            try
            {
                analysis.CreatedAt = DateTime.UtcNow;
                analysis.UpdatedAt = DateTime.UtcNow;

                _context.Analyses.Add(analysis);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = analysis.Id }, analysis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de l'analyse");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour une analyse
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Analysis analysis)
        {
            if (id != analysis.Id)
            {
                return BadRequest("L'ID ne correspond pas");
            }

            try
            {
                analysis.UpdatedAt = DateTime.UtcNow;
                _context.Entry(analysis).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AnalysisExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de l'analyse {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer une analyse
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var analysis = await _context.Analyses.FindAsync(id);
                if (analysis == null)
                {
                    return NotFound();
                }

                _context.Analyses.Remove(analysis);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de l'analyse {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task<bool> AnalysisExists(int id)
        {
            return await _context.Analyses.AnyAsync(e => e.Id == id);
        }
    }
}
