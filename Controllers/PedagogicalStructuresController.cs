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
    public class PedagogicalStructuresController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PedagogicalStructuresController> _logger;

        public PedagogicalStructuresController(AppDbContext context, ILogger<PedagogicalStructuresController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir toutes les structures pédagogiques
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PedagogicalStructure>>> GetAll()
        {
            try
            {
                return await _context.PedagogicalStructures
                    .OrderBy(s => s.SortOrder)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des structures pédagogiques");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir une structure pédagogique par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PedagogicalStructure>> GetById(int id)
        {
            try
            {
                var structure = await _context.PedagogicalStructures.FindAsync(id);
                if (structure == null)
                {
                    return NotFound();
                }
                return structure;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de la structure pédagogique {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les structures pédagogiques d'une organisation
        /// </summary>
        [HttpGet("organisation/{organisationId}")]
        public async Task<ActionResult<IEnumerable<PedagogicalStructure>>> GetByOrganisation(int organisationId)
        {
            try
            {
                return await _context.PedagogicalStructures
                    .Where(s => s.OrganisationId == organisationId)
                    .OrderBy(s => s.SortOrder)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des structures de l'organisation {OrganisationId}", organisationId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les structures pédagogiques par type
        /// </summary>
        [HttpGet("type/{typeId}")]
        public async Task<ActionResult<IEnumerable<PedagogicalStructure>>> GetByType(int typeId)
        {
            try
            {
                return await _context.PedagogicalStructures
                    .Where(s => s.PedagogicalElementTypeId == typeId)
                    .OrderBy(s => s.SortOrder)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des structures par type {TypeId}", typeId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les enfants d'une structure
        /// </summary>
        [HttpGet("{id}/children")]
        public async Task<ActionResult<IEnumerable<PedagogicalStructure>>> GetChildren(int id)
        {
            try
            {
                return await _context.PedagogicalStructures
                    .Where(s => s.ParentId == id)
                    .OrderBy(s => s.SortOrder)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des enfants de la structure {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les structures en arborescence
        /// </summary>
        [HttpGet("tree")]
        public async Task<ActionResult<IEnumerable<PedagogicalStructureTreeDto>>> GetTree()
        {
            try
            {
                var allStructures = await _context.PedagogicalStructures
                    .OrderBy(s => s.SortOrder)
                    .ToListAsync();

                var roots = allStructures.Where(s => s.ParentId == null);
                return Ok(BuildStructureTree(roots, allStructures));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'arbre des structures");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les structures en arborescence pour une organisation
        /// </summary>
        [HttpGet("organisation/{organisationId}/tree")]
        public async Task<ActionResult<IEnumerable<PedagogicalStructureTreeDto>>> GetTreeByOrganisation(int organisationId)
        {
            try
            {
                var allStructures = await _context.PedagogicalStructures
                    .Where(s => s.OrganisationId == organisationId)
                    .OrderBy(s => s.SortOrder)
                    .ToListAsync();

                var roots = allStructures.Where(s => s.ParentId == null);
                return Ok(BuildStructureTree(roots, allStructures));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'arbre pour l'organisation {OrganisationId}", organisationId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private List<PedagogicalStructureTreeDto> BuildStructureTree(IEnumerable<PedagogicalStructure> nodes, List<PedagogicalStructure> allStructures)
        {
            return nodes.Select(s => new PedagogicalStructureTreeDto
            {
                Id = s.Id,
                NameFr = s.NameFr,
                NameEn = s.NameEn,
                PedagogicalElementTypeId = s.PedagogicalElementTypeId,
                SectorCode = s.SectorCode,
                SortOrder = s.SortOrder,
                OrganisationId = s.OrganisationId,
                Children = BuildStructureTree(allStructures.Where(c => c.ParentId == s.Id), allStructures)
            }).ToList();
        }

        /// <summary>
        /// Créer une nouvelle structure pédagogique
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PedagogicalStructure>> Create(PedagogicalStructure structure)
        {
            try
            {
                structure.CreatedAt = DateTime.UtcNow;
                _context.PedagogicalStructures.Add(structure);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = structure.Id }, structure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de la structure pédagogique");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour une structure pédagogique
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PedagogicalStructure structure)
        {
            if (id != structure.Id)
            {
                return BadRequest("L'ID ne correspond pas");
            }

            try
            {
                _context.Entry(structure).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await StructureExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de la structure pédagogique {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer une structure pédagogique
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Vérifier s'il y a des enfants
                var hasChildren = await _context.PedagogicalStructures.AnyAsync(s => s.ParentId == id);
                if (hasChildren)
                {
                    return BadRequest("Impossible de supprimer une structure ayant des enfants");
                }

                var structure = await _context.PedagogicalStructures.FindAsync(id);
                if (structure == null)
                {
                    return NotFound();
                }

                _context.PedagogicalStructures.Remove(structure);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de la structure pédagogique {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task<bool> StructureExists(int id)
        {
            return await _context.PedagogicalStructures.AnyAsync(e => e.Id == id);
        }
    }
}
