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
    public class TechnologicalToolsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TechnologicalToolsController> _logger;

        public TechnologicalToolsController(AppDbContext context, ILogger<TechnologicalToolsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir tous les outils technologiques
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TechnologicalTool>>> GetAll()
        {
            try
            {
                return await _context.TechnologicalTools
                    .OrderBy(t => t.DisplayOrder)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des outils technologiques");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir un outil technologique par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TechnologicalTool>> GetById(int id)
        {
            try
            {
                var tool = await _context.TechnologicalTools.FindAsync(id);
                if (tool == null)
                {
                    return NotFound();
                }
                return tool;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'outil technologique {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les outils technologiques actifs
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<TechnologicalTool>>> GetActive()
        {
            try
            {
                return await _context.TechnologicalTools
                    .Where(t => t.IsActive)
                    .OrderBy(t => t.DisplayOrder)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des outils technologiques actifs");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Créer un nouvel outil technologique
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TechnologicalTool>> Create(TechnologicalTool tool)
        {
            try
            {
                tool.CreatedAt = DateTime.UtcNow;
                _context.TechnologicalTools.Add(tool);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = tool.Id }, tool);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de l'outil technologique");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour un outil technologique
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TechnologicalTool tool)
        {
            if (id != tool.Id)
            {
                return BadRequest("L'ID ne correspond pas");
            }

            try
            {
                tool.UpdatedAt = DateTime.UtcNow;
                _context.Entry(tool).State = EntityState.Modified;
                // Ne pas modifier la date de création
                _context.Entry(tool).Property(x => x.CreatedAt).IsModified = false;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ToolExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de l'outil technologique {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer un outil technologique
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var tool = await _context.TechnologicalTools.FindAsync(id);
                if (tool == null)
                {
                    return NotFound();
                }

                _context.TechnologicalTools.Remove(tool);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de l'outil technologique {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task<bool> ToolExists(int id)
        {
            return await _context.TechnologicalTools.AnyAsync(e => e.Id == id);
        }
    }
}
