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
    public class ValueDomainItemsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ValueDomainItemsController> _logger;

        public ValueDomainItemsController(AppDbContext context, ILogger<ValueDomainItemsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir tous les items de domaines de valeurs
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ValueDomainItem>>> GetAll()
        {
            try
            {
                return await _context.ValueDomainItems
                    .OrderBy(i => i.ValueDomainId)
                    .ThenBy(i => i.Order)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des items");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir un item par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ValueDomainItem>> GetById(int id)
        {
            try
            {
                var item = await _context.ValueDomainItems.FindAsync(id);
                if (item == null)
                {
                    return NotFound();
                }
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'item {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les items d'un domaine de valeurs
        /// </summary>
        [HttpGet("domain/{domainId}")]
        public async Task<ActionResult<IEnumerable<ValueDomainItem>>> GetByDomain(int domainId)
        {
            try
            {
                return await _context.ValueDomainItems
                    .Where(i => i.ValueDomainId == domainId)
                    .OrderBy(i => i.Order)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des items du domaine {DomainId}", domainId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Créer un nouvel item
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ValueDomainItem>> Create(ValueDomainItem item)
        {
            try
            {
                // Vérifier que le domaine existe
                if (!await _context.ValueDomains.AnyAsync(d => d.Id == item.ValueDomainId))
                {
                    return BadRequest("Le domaine de valeurs spécifié n'existe pas");
                }

                // Si l'ordre n'est pas spécifié, mettre à la fin
                if (item.Order == 0)
                {
                    var maxOrder = await _context.ValueDomainItems
                        .Where(i => i.ValueDomainId == item.ValueDomainId)
                        .MaxAsync(i => (int?)i.Order) ?? 0;
                    item.Order = maxOrder + 1;
                }

                _context.ValueDomainItems.Add(item);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de l'item");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour un item
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ValueDomainItem item)
        {
            if (id != item.Id)
            {
                return BadRequest("L'ID ne correspond pas");
            }

            try
            {
                _context.Entry(item).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ItemExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de l'item {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Réorganiser les items d'un domaine
        /// </summary>
        [HttpPut("domain/{domainId}/reorder")]
        public async Task<IActionResult> Reorder(int domainId, [FromBody] List<int> itemIds)
        {
            try
            {
                var items = await _context.ValueDomainItems
                    .Where(i => i.ValueDomainId == domainId)
                    .ToListAsync();

                for (int i = 0; i < itemIds.Count; i++)
                {
                    var item = items.FirstOrDefault(x => x.Id == itemIds[i]);
                    if (item != null)
                    {
                        item.Order = i + 1;
                    }
                }

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la réorganisation des items du domaine {DomainId}", domainId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer un item
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var item = await _context.ValueDomainItems.FindAsync(id);
                if (item == null)
                {
                    return NotFound();
                }

                _context.ValueDomainItems.Remove(item);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de l'item {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task<bool> ItemExists(int id)
        {
            return await _context.ValueDomainItems.AnyAsync(e => e.Id == id);
        }
    }
}
