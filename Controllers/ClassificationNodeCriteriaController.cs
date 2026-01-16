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
    public class ClassificationNodeCriteriaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ClassificationNodeCriteriaController> _logger;

        public ClassificationNodeCriteriaController(AppDbContext context, ILogger<ClassificationNodeCriteriaController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir tous les critères
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClassificationNodeCriteria>>> GetAll()
        {
            try
            {
                return await _context.ClassificationNodeCriteria
                    .OrderBy(c => c.SortOrder)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des critères");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir un critère par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ClassificationNodeCriteria>> GetById(int id)
        {
            try
            {
                var criteria = await _context.ClassificationNodeCriteria.FindAsync(id);
                if (criteria == null)
                {
                    return NotFound();
                }
                return criteria;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du critère {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les critères d'un noeud
        /// </summary>
        [HttpGet("node/{nodeId}")]
        public async Task<ActionResult<IEnumerable<ClassificationNodeCriteria>>> GetByNode(int nodeId)
        {
            try
            {
                return await _context.ClassificationNodeCriteria
                    .Where(c => c.ClassificationNodeId == nodeId)
                    .OrderBy(c => c.SortOrder)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des critères du noeud {NodeId}", nodeId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Créer un nouveau critère
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ClassificationNodeCriteria>> Create(ClassificationNodeCriteria criteria)
        {
            try
            {
                _context.ClassificationNodeCriteria.Add(criteria);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = criteria.Id }, criteria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création du critère");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour un critère
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ClassificationNodeCriteria criteria)
        {
            if (id != criteria.Id)
            {
                return BadRequest("L'ID ne correspond pas");
            }

            try
            {
                _context.Entry(criteria).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CriteriaExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour du critère {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer un critère
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var criteria = await _context.ClassificationNodeCriteria.FindAsync(id);
                if (criteria == null)
                {
                    return NotFound();
                }

                _context.ClassificationNodeCriteria.Remove(criteria);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression du critère {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task<bool> CriteriaExists(int id)
        {
            return await _context.ClassificationNodeCriteria.AnyAsync(e => e.Id == id);
        }
    }
}
