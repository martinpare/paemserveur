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
    public class ModificationsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ModificationsController> _logger;

        public ModificationsController(AppDbContext context, ILogger<ModificationsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir toutes les modifications
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Modification>>> GetAll()
        {
            try
            {
                return await _context.Modifications
                    .OrderByDescending(m => m.Date)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des modifications");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir une modification par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Modification>> GetById(int id)
        {
            try
            {
                var modification = await _context.Modifications.FindAsync(id);
                if (modification == null)
                {
                    return NotFound();
                }
                return modification;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de la modification {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les modifications d'un item
        /// </summary>
        [HttpGet("item/{itemId}")]
        public async Task<ActionResult<IEnumerable<Modification>>> GetByItem(int itemId)
        {
            try
            {
                return await _context.Modifications
                    .Where(m => m.ItemId == itemId)
                    .OrderByDescending(m => m.Date)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des modifications de l'item {ItemId}", itemId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Créer une nouvelle modification
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Modification>> Create(Modification modification)
        {
            try
            {
                modification.CreatedAt = DateTime.UtcNow;
                modification.UpdatedAt = DateTime.UtcNow;

                _context.Modifications.Add(modification);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = modification.Id }, modification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de la modification");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour une modification
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Modification modification)
        {
            if (id != modification.Id)
            {
                return BadRequest("L'ID ne correspond pas");
            }

            try
            {
                modification.UpdatedAt = DateTime.UtcNow;
                _context.Entry(modification).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ModificationExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de la modification {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer une modification
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var modification = await _context.Modifications.FindAsync(id);
                if (modification == null)
                {
                    return NotFound();
                }

                _context.Modifications.Remove(modification);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de la modification {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task<bool> ModificationExists(int id)
        {
            return await _context.Modifications.AnyAsync(e => e.Id == id);
        }
    }
}
