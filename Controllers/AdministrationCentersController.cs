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
    public class AdministrationCentersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AdministrationCentersController> _logger;

        public AdministrationCentersController(AppDbContext context, ILogger<AdministrationCentersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir tous les centres administratifs
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdministrationCenter>>> GetAll()
        {
            try
            {
                return await _context.AdministrationCenters
                    .OrderBy(c => c.ShortName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des centres administratifs");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir un centre administratif par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<AdministrationCenter>> GetById(int id)
        {
            try
            {
                var center = await _context.AdministrationCenters.FindAsync(id);
                if (center == null)
                {
                    return NotFound();
                }
                return center;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du centre administratif {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir un centre administratif par son code
        /// </summary>
        [HttpGet("code/{code}")]
        public async Task<ActionResult<AdministrationCenter>> GetByCode(string code)
        {
            try
            {
                var center = await _context.AdministrationCenters
                    .FirstOrDefaultAsync(c => c.Code == code);
                if (center == null)
                {
                    return NotFound();
                }
                return center;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du centre par code {Code}", code);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les centres d'une organisation
        /// </summary>
        [HttpGet("organisation/{organisationId}")]
        public async Task<ActionResult<IEnumerable<AdministrationCenter>>> GetByOrganisation(int organisationId)
        {
            try
            {
                return await _context.AdministrationCenters
                    .Where(c => c.OrganisationId == organisationId)
                    .OrderBy(c => c.ShortName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des centres de l'organisation {OrganisationId}", organisationId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Créer un nouveau centre administratif
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<AdministrationCenter>> Create(AdministrationCenter center)
        {
            try
            {
                _context.AdministrationCenters.Add(center);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = center.Id }, center);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création du centre administratif");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour un centre administratif
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, AdministrationCenter center)
        {
            if (id != center.Id)
            {
                return BadRequest("L'ID ne correspond pas");
            }

            try
            {
                _context.Entry(center).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CenterExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour du centre administratif {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer un centre administratif
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var center = await _context.AdministrationCenters.FindAsync(id);
                if (center == null)
                {
                    return NotFound();
                }

                _context.AdministrationCenters.Remove(center);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression du centre administratif {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task<bool> CenterExists(int id)
        {
            return await _context.AdministrationCenters.AnyAsync(e => e.Id == id);
        }
    }
}
