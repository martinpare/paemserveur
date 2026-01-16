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
    public class RevisionsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<RevisionsController> _logger;

        public RevisionsController(AppDbContext context, ILogger<RevisionsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir toutes les révisions
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Revision>>> GetAll()
        {
            try
            {
                return await _context.Revisions
                    .OrderByDescending(r => r.Date)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des révisions");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir une révision par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Revision>> GetById(int id)
        {
            try
            {
                var revision = await _context.Revisions.FindAsync(id);
                if (revision == null)
                {
                    return NotFound();
                }
                return revision;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de la révision {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les révisions d'un item
        /// </summary>
        [HttpGet("item/{itemId}")]
        public async Task<ActionResult<IEnumerable<Revision>>> GetByItem(int itemId)
        {
            try
            {
                return await _context.Revisions
                    .Where(r => r.ItemId == itemId)
                    .OrderByDescending(r => r.Date)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des révisions de l'item {ItemId}", itemId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Créer une nouvelle révision
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Revision>> Create(Revision revision)
        {
            try
            {
                revision.CreatedAt = DateTime.UtcNow;
                revision.UpdatedAt = DateTime.UtcNow;

                _context.Revisions.Add(revision);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = revision.Id }, revision);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de la révision");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour une révision
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Revision revision)
        {
            if (id != revision.Id)
            {
                return BadRequest("L'ID ne correspond pas");
            }

            try
            {
                revision.UpdatedAt = DateTime.UtcNow;
                _context.Entry(revision).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await RevisionExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de la révision {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer une révision
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var revision = await _context.Revisions.FindAsync(id);
                if (revision == null)
                {
                    return NotFound();
                }

                _context.Revisions.Remove(revision);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de la révision {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task<bool> RevisionExists(int id)
        {
            return await _context.Revisions.AnyAsync(e => e.Id == id);
        }
    }
}
