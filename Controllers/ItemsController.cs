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
    public class ItemsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ItemsController> _logger;

        public ItemsController(AppDbContext context, ILogger<ItemsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir tous les items
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Item>>> GetAll()
        {
            try
            {
                return await _context.Items
                    .OrderByDescending(i => i.UpdatedAt)
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
        public async Task<ActionResult<Item>> GetById(int id)
        {
            try
            {
                var item = await _context.Items.FindAsync(id);
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
        /// Obtenir les items d'une banque
        /// </summary>
        [HttpGet("bank/{bankId}")]
        public async Task<ActionResult<IEnumerable<Item>>> GetByBank(int bankId)
        {
            try
            {
                return await _context.Items
                    .Where(i => i.ItemBankId == bankId)
                    .OrderBy(i => i.Number)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des items de la banque {BankId}", bankId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les items d'un document
        /// </summary>
        [HttpGet("document/{documentId}")]
        public async Task<ActionResult<IEnumerable<Item>>> GetByDocument(int documentId)
        {
            try
            {
                return await _context.Items
                    .Where(i => i.DocumentId == documentId)
                    .OrderBy(i => i.Number)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des items du document {DocumentId}", documentId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les items d'un utilisateur
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Item>>> GetByUser(int userId)
        {
            try
            {
                return await _context.Items
                    .Where(i => i.UserId == userId)
                    .OrderByDescending(i => i.UpdatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des items de l'utilisateur {UserId}", userId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les items par type
        /// </summary>
        [HttpGet("type/{typeId}")]
        public async Task<ActionResult<IEnumerable<Item>>> GetByType(int typeId)
        {
            try
            {
                return await _context.Items
                    .Where(i => i.TypeId == typeId)
                    .OrderByDescending(i => i.UpdatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des items par type {TypeId}", typeId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les items par statut
        /// </summary>
        [HttpGet("status/{statusId}")]
        public async Task<ActionResult<IEnumerable<Item>>> GetByStatus(int statusId)
        {
            try
            {
                return await _context.Items
                    .Where(i => i.StatusId == statusId)
                    .OrderByDescending(i => i.UpdatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des items par statut {StatusId}", statusId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Créer un nouvel item
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Item>> Create(Item item)
        {
            try
            {
                item.CreatedAt = DateTime.UtcNow;
                item.UpdatedAt = DateTime.UtcNow;

                _context.Items.Add(item);
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
        public async Task<IActionResult> Update(int id, Item item)
        {
            if (id != item.Id)
            {
                return BadRequest("L'ID ne correspond pas");
            }

            try
            {
                item.UpdatedAt = DateTime.UtcNow;
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
        /// Supprimer un item
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var item = await _context.Items.FindAsync(id);
                if (item == null)
                {
                    return NotFound();
                }

                _context.Items.Remove(item);
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
            return await _context.Items.AnyAsync(e => e.Id == id);
        }
    }
}
