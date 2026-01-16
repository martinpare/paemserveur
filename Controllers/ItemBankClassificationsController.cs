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
    public class ItemBankClassificationsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ItemBankClassificationsController> _logger;

        public ItemBankClassificationsController(AppDbContext context, ILogger<ItemBankClassificationsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir toutes les associations banque d'items - classifications
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemBankClassification>>> GetAll()
        {
            try
            {
                return await _context.ItemBankClassifications
                    .OrderByDescending(i => i.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des associations banque d'items - classifications");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir une association par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemBankClassification>> GetById(int id)
        {
            try
            {
                var itemBankClassification = await _context.ItemBankClassifications.FindAsync(id);
                if (itemBankClassification == null)
                {
                    return NotFound();
                }
                return itemBankClassification;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'association {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les classifications d'une banque d'items
        /// </summary>
        [HttpGet("itembank/{itemBankId}")]
        public async Task<ActionResult<IEnumerable<ItemBankClassification>>> GetByItemBank(int itemBankId)
        {
            try
            {
                return await _context.ItemBankClassifications
                    .Where(i => i.ItemBankId == itemBankId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des classifications de la banque d'items {ItemBankId}", itemBankId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les banques d'items d'une classification
        /// </summary>
        [HttpGet("classification/{classificationId}")]
        public async Task<ActionResult<IEnumerable<ItemBankClassification>>> GetByClassification(int classificationId)
        {
            try
            {
                return await _context.ItemBankClassifications
                    .Where(i => i.ClassificationId == classificationId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des banques d'items de la classification {ClassificationId}", classificationId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Créer une nouvelle association
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ItemBankClassification>> Create(ItemBankClassification itemBankClassification)
        {
            try
            {
                itemBankClassification.CreatedAt = DateTime.UtcNow;
                itemBankClassification.UpdatedAt = DateTime.UtcNow;

                _context.ItemBankClassifications.Add(itemBankClassification);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = itemBankClassification.Id }, itemBankClassification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de l'association");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour une association
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ItemBankClassification itemBankClassification)
        {
            if (id != itemBankClassification.Id)
            {
                return BadRequest("L'ID ne correspond pas");
            }

            try
            {
                itemBankClassification.UpdatedAt = DateTime.UtcNow;
                _context.Entry(itemBankClassification).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ItemBankClassificationExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de l'association {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer une association
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var itemBankClassification = await _context.ItemBankClassifications.FindAsync(id);
                if (itemBankClassification == null)
                {
                    return NotFound();
                }

                _context.ItemBankClassifications.Remove(itemBankClassification);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de l'association {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task<bool> ItemBankClassificationExists(int id)
        {
            return await _context.ItemBankClassifications.AnyAsync(e => e.Id == id);
        }
    }
}
