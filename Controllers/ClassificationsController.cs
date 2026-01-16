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
    public class ClassificationsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ClassificationsController> _logger;

        public ClassificationsController(AppDbContext context, ILogger<ClassificationsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir toutes les classifications
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Classification>>> GetAll()
        {
            try
            {
                return await _context.Classifications.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des classifications");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir une classification par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Classification>> GetById(int id)
        {
            try
            {
                var classification = await _context.Classifications.FindAsync(id);
                if (classification == null)
                {
                    return NotFound();
                }
                return classification;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de la classification {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir une classification par son tag
        /// </summary>
        [HttpGet("tag/{tag}")]
        public async Task<ActionResult<Classification>> GetByTag(string tag)
        {
            try
            {
                var classification = await _context.Classifications
                    .FirstOrDefaultAsync(c => c.Tag == tag);
                if (classification == null)
                {
                    return NotFound();
                }
                return classification;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de la classification par tag {Tag}", tag);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les classifications d'une structure pédagogique
        /// </summary>
        [HttpGet("pedagogicalstructure/{structureId}")]
        public async Task<ActionResult<IEnumerable<Classification>>> GetByStructure(int structureId)
        {
            try
            {
                return await _context.Classifications
                    .Where(c => c.PedagogicalStructureId == structureId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des classifications de la structure {StructureId}", structureId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les classifications actives
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Classification>>> GetActive()
        {
            try
            {
                return await _context.Classifications
                    .Where(c => c.IsActive)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des classifications actives");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Créer une nouvelle classification
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Classification>> Create(Classification classification)
        {
            try
            {
                _context.Classifications.Add(classification);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = classification.Id }, classification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de la classification");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour une classification
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Classification classification)
        {
            if (id != classification.Id)
            {
                return BadRequest("L'ID ne correspond pas");
            }

            try
            {
                _context.Entry(classification).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ClassificationExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de la classification {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer une classification
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Vérifier s'il y a des noeuds
                var hasNodes = await _context.ClassificationNodes.AnyAsync(n => n.ClassificationId == id);
                if (hasNodes)
                {
                    return BadRequest("Impossible de supprimer une classification contenant des noeuds");
                }

                var classification = await _context.Classifications.FindAsync(id);
                if (classification == null)
                {
                    return NotFound();
                }

                _context.Classifications.Remove(classification);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de la classification {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task<bool> ClassificationExists(int id)
        {
            return await _context.Classifications.AnyAsync(e => e.Id == id);
        }
    }
}
