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
    public class ItemVersionsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ItemVersionsController> _logger;

        public ItemVersionsController(AppDbContext context, ILogger<ItemVersionsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir toutes les versions d'items
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemVersion>>> GetAll()
        {
            try
            {
                return await _context.ItemVersions
                    .OrderByDescending(i => i.Date)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des versions d'items");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir une version d'item par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemVersion>> GetById(int id)
        {
            try
            {
                var itemVersion = await _context.ItemVersions.FindAsync(id);
                if (itemVersion == null)
                {
                    return NotFound();
                }
                return itemVersion;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de la version d'item {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les versions d'un item
        /// </summary>
        [HttpGet("item/{itemId}")]
        public async Task<ActionResult<IEnumerable<ItemVersion>>> GetByItem(int itemId)
        {
            try
            {
                return await _context.ItemVersions
                    .Where(i => i.ItemId == itemId)
                    .OrderByDescending(i => i.Version)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des versions de l'item {ItemId}", itemId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir la dernière version d'un item
        /// </summary>
        [HttpGet("item/{itemId}/latest")]
        public async Task<ActionResult<ItemVersion>> GetLatestByItem(int itemId)
        {
            try
            {
                var itemVersion = await _context.ItemVersions
                    .Where(i => i.ItemId == itemId)
                    .OrderByDescending(i => i.Version)
                    .FirstOrDefaultAsync();

                if (itemVersion == null)
                {
                    return NotFound();
                }
                return itemVersion;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de la dernière version de l'item {ItemId}", itemId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Créer une nouvelle version d'item
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ItemVersion>> Create(ItemVersion itemVersion)
        {
            try
            {
                itemVersion.CreatedAt = DateTime.UtcNow;
                itemVersion.UpdatedAt = DateTime.UtcNow;

                _context.ItemVersions.Add(itemVersion);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = itemVersion.Id }, itemVersion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de la version d'item");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour une version d'item
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ItemVersion itemVersion)
        {
            if (id != itemVersion.Id)
            {
                return BadRequest("L'ID ne correspond pas");
            }

            try
            {
                itemVersion.UpdatedAt = DateTime.UtcNow;
                _context.Entry(itemVersion).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ItemVersionExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de la version d'item {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer une version d'item
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemVersion = await _context.ItemVersions.FindAsync(id);
                if (itemVersion == null)
                {
                    return NotFound();
                }

                _context.ItemVersions.Remove(itemVersion);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de la version d'item {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task<bool> ItemVersionExists(int id)
        {
            return await _context.ItemVersions.AnyAsync(e => e.Id == id);
        }
    }
}
