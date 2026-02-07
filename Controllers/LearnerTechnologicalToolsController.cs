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
    public class LearnerTechnologicalToolsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<LearnerTechnologicalToolsController> _logger;

        public LearnerTechnologicalToolsController(AppDbContext context, ILogger<LearnerTechnologicalToolsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir toutes les assignations apprenant-outil technologique
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LearnerTechnologicalTool>>> GetAll()
        {
            try
            {
                return await _context.LearnerTechnologicalTools.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des assignations apprenant-outil");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir une assignation par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<LearnerTechnologicalTool>> GetById(int id)
        {
            try
            {
                var item = await _context.LearnerTechnologicalTools.FindAsync(id);
                if (item == null)
                {
                    return NotFound();
                }
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'assignation {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les outils technologiques d'un apprenant
        /// </summary>
        [HttpGet("learner/{learnerId}")]
        public async Task<ActionResult<IEnumerable<LearnerTechnologicalTool>>> GetByLearner(int learnerId)
        {
            try
            {
                return await _context.LearnerTechnologicalTools
                    .Where(ltt => ltt.LearnerId == learnerId)
                    .Include(ltt => ltt.TechnologicalTool)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des outils de l'apprenant {LearnerId}", learnerId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les apprenants ayant un outil technologique
        /// </summary>
        [HttpGet("tool/{technologicalToolId}")]
        public async Task<ActionResult<IEnumerable<LearnerTechnologicalTool>>> GetByTool(int technologicalToolId)
        {
            try
            {
                return await _context.LearnerTechnologicalTools
                    .Where(ltt => ltt.TechnologicalToolId == technologicalToolId)
                    .Include(ltt => ltt.Learner)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des apprenants pour l'outil {ToolId}", technologicalToolId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Assigner un outil technologique à un apprenant
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<LearnerTechnologicalTool>> Create(LearnerTechnologicalTool item)
        {
            try
            {
                // Vérifier si l'assignation existe déjà
                var exists = await _context.LearnerTechnologicalTools
                    .AnyAsync(ltt => ltt.LearnerId == item.LearnerId &&
                                     ltt.TechnologicalToolId == item.TechnologicalToolId);

                if (exists)
                {
                    return BadRequest("Cet apprenant a déjà cet outil technologique assigné");
                }

                item.CreatedAt = DateTime.UtcNow;
                _context.LearnerTechnologicalTools.Add(item);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'assignation de l'outil technologique");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Synchroniser les outils technologiques d'un apprenant (remplace toutes les assignations existantes)
        /// </summary>
        [HttpPut("learner/{learnerId}/sync")]
        public async Task<IActionResult> SyncLearnerTools(int learnerId, [FromBody] SyncLearnerToolsRequest request)
        {
            try
            {
                // Supprimer les anciennes assignations
                var existingAssignments = await _context.LearnerTechnologicalTools
                    .Where(ltt => ltt.LearnerId == learnerId)
                    .ToListAsync();
                _context.LearnerTechnologicalTools.RemoveRange(existingAssignments);

                // Créer les nouvelles assignations
                if (request.TechnologicalToolIds != null && request.TechnologicalToolIds.Any())
                {
                    var newAssignments = request.TechnologicalToolIds.Select(toolId => new LearnerTechnologicalTool
                    {
                        LearnerId = learnerId,
                        TechnologicalToolId = toolId,
                        CreatedAt = DateTime.UtcNow
                    });
                    _context.LearnerTechnologicalTools.AddRange(newAssignments);
                }

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la synchronisation des outils pour l'apprenant {LearnerId}", learnerId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour les données personnalisées d'un outil pour un apprenant
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, LearnerTechnologicalTool item)
        {
            if (id != item.Id)
            {
                return BadRequest();
            }

            try
            {
                _context.Entry(item).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.LearnerTechnologicalTools.AnyAsync(e => e.Id == id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de l'assignation {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer une assignation par son ID
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var item = await _context.LearnerTechnologicalTools.FindAsync(id);
                if (item == null)
                {
                    return NotFound();
                }

                _context.LearnerTechnologicalTools.Remove(item);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de l'assignation {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer une assignation par apprenant et outil
        /// </summary>
        [HttpDelete("learner/{learnerId}/tool/{technologicalToolId}")]
        public async Task<IActionResult> DeleteByComposite(int learnerId, int technologicalToolId)
        {
            try
            {
                var item = await _context.LearnerTechnologicalTools
                    .FirstOrDefaultAsync(ltt => ltt.LearnerId == learnerId && ltt.TechnologicalToolId == technologicalToolId);

                if (item == null)
                {
                    return NotFound();
                }

                _context.LearnerTechnologicalTools.Remove(item);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de l'assignation apprenant {LearnerId} / outil {ToolId}", learnerId, technologicalToolId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }
    }

    public class SyncLearnerToolsRequest
    {
        public List<int> TechnologicalToolIds { get; set; }
    }
}
