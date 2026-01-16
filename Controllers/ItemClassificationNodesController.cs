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
    public class ItemClassificationNodesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ItemClassificationNodesController> _logger;

        public ItemClassificationNodesController(AppDbContext context, ILogger<ItemClassificationNodesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir toutes les associations item - noeud de classification
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemClassificationNode>>> GetAll()
        {
            try
            {
                return await _context.ItemClassificationNodes
                    .OrderByDescending(i => i.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des associations item - noeud de classification");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir une association par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemClassificationNode>> GetById(int id)
        {
            try
            {
                var itemClassificationNode = await _context.ItemClassificationNodes.FindAsync(id);
                if (itemClassificationNode == null)
                {
                    return NotFound();
                }
                return itemClassificationNode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'association {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les noeuds de classification d'un item
        /// </summary>
        [HttpGet("item/{itemId}")]
        public async Task<ActionResult<IEnumerable<ItemClassificationNode>>> GetByItem(int itemId)
        {
            try
            {
                return await _context.ItemClassificationNodes
                    .Where(i => i.ItemId == itemId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des noeuds de classification de l'item {ItemId}", itemId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les items d'un noeud de classification
        /// </summary>
        [HttpGet("classificationnode/{classificationNodeId}")]
        public async Task<ActionResult<IEnumerable<ItemClassificationNode>>> GetByClassificationNode(int classificationNodeId)
        {
            try
            {
                return await _context.ItemClassificationNodes
                    .Where(i => i.ClassificationNodeId == classificationNodeId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des items du noeud de classification {ClassificationNodeId}", classificationNodeId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Créer une nouvelle association
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ItemClassificationNode>> Create(ItemClassificationNode itemClassificationNode)
        {
            try
            {
                itemClassificationNode.CreatedAt = DateTime.UtcNow;
                itemClassificationNode.UpdatedAt = DateTime.UtcNow;

                _context.ItemClassificationNodes.Add(itemClassificationNode);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = itemClassificationNode.Id }, itemClassificationNode);
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
        public async Task<IActionResult> Update(int id, ItemClassificationNode itemClassificationNode)
        {
            if (id != itemClassificationNode.Id)
            {
                return BadRequest("L'ID ne correspond pas");
            }

            try
            {
                itemClassificationNode.UpdatedAt = DateTime.UtcNow;
                _context.Entry(itemClassificationNode).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ItemClassificationNodeExists(id))
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
                var itemClassificationNode = await _context.ItemClassificationNodes.FindAsync(id);
                if (itemClassificationNode == null)
                {
                    return NotFound();
                }

                _context.ItemClassificationNodes.Remove(itemClassificationNode);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de l'association {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task<bool> ItemClassificationNodeExists(int id)
        {
            return await _context.ItemClassificationNodes.AnyAsync(e => e.Id == id);
        }
    }
}
