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
    public class PedagogicalElementTypesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PedagogicalElementTypesController> _logger;

        public PedagogicalElementTypesController(AppDbContext context, ILogger<PedagogicalElementTypesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir tous les types d'éléments pédagogiques
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PedagogicalElementType>>> GetAll()
        {
            try
            {
                return await _context.PedagogicalElementTypes.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des types d'éléments pédagogiques");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir un type d'élément pédagogique par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PedagogicalElementType>> GetById(int id)
        {
            try
            {
                var type = await _context.PedagogicalElementTypes.FindAsync(id);
                if (type == null)
                {
                    return NotFound();
                }
                return type;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du type d'élément pédagogique {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les types d'éléments pédagogiques d'une organisation
        /// </summary>
        [HttpGet("organisation/{organisationId}")]
        public async Task<ActionResult<IEnumerable<PedagogicalElementType>>> GetByOrganisation(int organisationId)
        {
            try
            {
                return await _context.PedagogicalElementTypes
                    .Where(t => t.OrganisationId == organisationId || t.OrganisationId == null)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des types pour l'organisation {OrganisationId}", organisationId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Créer un nouveau type d'élément pédagogique
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PedagogicalElementType>> Create(PedagogicalElementType type)
        {
            try
            {
                _context.PedagogicalElementTypes.Add(type);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = type.Id }, type);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création du type d'élément pédagogique");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour un type d'élément pédagogique
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PedagogicalElementType type)
        {
            if (id != type.Id)
            {
                return BadRequest("L'ID ne correspond pas");
            }

            try
            {
                _context.Entry(type).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TypeExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour du type d'élément pédagogique {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer un type d'élément pédagogique
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Vérifier s'il y a des structures utilisant ce type
                var hasStructures = await _context.PedagogicalStructures.AnyAsync(s => s.PedagogicalElementTypeId == id);
                if (hasStructures)
                {
                    return BadRequest("Impossible de supprimer un type utilisé par des structures pédagogiques");
                }

                var type = await _context.PedagogicalElementTypes.FindAsync(id);
                if (type == null)
                {
                    return NotFound();
                }

                _context.PedagogicalElementTypes.Remove(type);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression du type d'élément pédagogique {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task<bool> TypeExists(int id)
        {
            return await _context.PedagogicalElementTypes.AnyAsync(e => e.Id == id);
        }
    }
}
