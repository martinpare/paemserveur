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
    public class ItemBanksController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ItemBanksController> _logger;

        public ItemBanksController(AppDbContext context, ILogger<ItemBanksController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir toutes les banques d'items
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemBank>>> GetAll()
        {
            try
            {
                return await _context.ItemBanks.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des banques d'items");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir une banque par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemBank>> GetById(int id)
        {
            try
            {
                var bank = await _context.ItemBanks.FindAsync(id);
                if (bank == null)
                {
                    return NotFound();
                }
                return bank;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de la banque {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les banques d'une structure pédagogique
        /// </summary>
        [HttpGet("pedagogicalstructure/{structureId}")]
        public async Task<ActionResult<IEnumerable<ItemBank>>> GetByStructure(int structureId)
        {
            try
            {
                return await _context.ItemBanks
                    .Where(b => b.PedagogicalStructureId == structureId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des banques de la structure {StructureId}", structureId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les banques actives
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<ItemBank>>> GetActive()
        {
            try
            {
                return await _context.ItemBanks
                    .Where(b => b.IsActive == true)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des banques actives");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Créer une nouvelle banque
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ItemBank>> Create(ItemBank bank)
        {
            try
            {
                _context.ItemBanks.Add(bank);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = bank.Id }, bank);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de la banque");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour une banque
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ItemBank bank)
        {
            if (id != bank.Id)
            {
                return BadRequest("L'ID ne correspond pas");
            }

            try
            {
                _context.Entry(bank).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await BankExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de la banque {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer une banque
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Vérifier s'il y a des items dans cette banque
                var hasItems = await _context.Items.AnyAsync(i => i.ItemBankId == id);
                if (hasItems)
                {
                    return BadRequest("Impossible de supprimer une banque contenant des items");
                }

                var bank = await _context.ItemBanks.FindAsync(id);
                if (bank == null)
                {
                    return NotFound();
                }

                _context.ItemBanks.Remove(bank);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de la banque {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task<bool> BankExists(int id)
        {
            return await _context.ItemBanks.AnyAsync(e => e.Id == id);
        }
    }
}
