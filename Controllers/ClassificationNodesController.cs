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
using serveur.Models.Dtos;

namespace serveur.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClassificationNodesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ClassificationNodesController> _logger;

        public ClassificationNodesController(AppDbContext context, ILogger<ClassificationNodesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir tous les noeuds de classification
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClassificationNode>>> GetAll()
        {
            try
            {
                return await _context.ClassificationNodes
                    .OrderBy(n => n.SortOrder)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des noeuds de classification");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir un noeud par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ClassificationNode>> GetById(int id)
        {
            try
            {
                var node = await _context.ClassificationNodes.FindAsync(id);
                if (node == null)
                {
                    return NotFound();
                }
                return node;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du noeud {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les noeuds d'une classification
        /// </summary>
        [HttpGet("classification/{classificationId}")]
        public async Task<ActionResult<IEnumerable<ClassificationNode>>> GetByClassification(int classificationId)
        {
            try
            {
                return await _context.ClassificationNodes
                    .Where(n => n.ClassificationId == classificationId)
                    .OrderBy(n => n.SortOrder)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des noeuds de la classification {ClassificationId}", classificationId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les enfants d'un noeud
        /// </summary>
        [HttpGet("{id}/children")]
        public async Task<ActionResult<IEnumerable<ClassificationNode>>> GetChildren(int id)
        {
            try
            {
                return await _context.ClassificationNodes
                    .Where(n => n.ParentId == id)
                    .OrderBy(n => n.SortOrder)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des enfants du noeud {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les noeuds en arborescence pour une classification
        /// </summary>
        [HttpGet("classification/{classificationId}/tree")]
        public async Task<ActionResult<IEnumerable<ClassificationNodeTreeDto>>> GetTree(int classificationId)
        {
            try
            {
                var allNodes = await _context.ClassificationNodes
                    .Where(n => n.ClassificationId == classificationId)
                    .OrderBy(n => n.SortOrder)
                    .ToListAsync();

                var roots = allNodes.Where(n => n.ParentId == null);
                return Ok(BuildNodeTree(roots, allNodes));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'arbre pour la classification {ClassificationId}", classificationId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private List<ClassificationNodeTreeDto> BuildNodeTree(IEnumerable<ClassificationNode> nodes, List<ClassificationNode> allNodes)
        {
            return nodes.Select(n => new ClassificationNodeTreeDto
            {
                Id = n.Id,
                Label = n.Label,
                NameFr = n.NameFr,
                NameEn = n.NameEn,
                DescriptionFr = n.DescriptionFr,
                DescriptionEn = n.DescriptionEn,
                SortOrder = n.SortOrder,
                Weight = n.Weight,
                ReferencesJuridiques = n.ReferencesJuridiques,
                Children = BuildNodeTree(allNodes.Where(c => c.ParentId == n.Id), allNodes)
            }).ToList();
        }

        /// <summary>
        /// Créer un nouveau noeud
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ClassificationNode>> Create(ClassificationNode node)
        {
            try
            {
                _context.ClassificationNodes.Add(node);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = node.Id }, node);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création du noeud");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour un noeud
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ClassificationNode node)
        {
            if (id != node.Id)
            {
                return BadRequest("L'ID ne correspond pas");
            }

            try
            {
                _context.Entry(node).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await NodeExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour du noeud {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer un noeud
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Vérifier s'il y a des enfants
                var hasChildren = await _context.ClassificationNodes.AnyAsync(n => n.ParentId == id);
                if (hasChildren)
                {
                    return BadRequest("Impossible de supprimer un noeud ayant des enfants");
                }

                // Vérifier s'il y a des critères
                var hasCriteria = await _context.ClassificationNodeCriteria.AnyAsync(c => c.ClassificationNodeId == id);
                if (hasCriteria)
                {
                    return BadRequest("Impossible de supprimer un noeud ayant des critères");
                }

                var node = await _context.ClassificationNodes.FindAsync(id);
                if (node == null)
                {
                    return NotFound();
                }

                _context.ClassificationNodes.Remove(node);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression du noeud {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task<bool> NodeExists(int id)
        {
            return await _context.ClassificationNodes.AnyAsync(e => e.Id == id);
        }
    }
}
