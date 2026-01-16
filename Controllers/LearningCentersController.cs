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
    public class LearningCentersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<LearningCentersController> _logger;

        public LearningCentersController(AppDbContext context, ILogger<LearningCentersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir tous les centres d'apprentissage
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LearningCenter>>> GetAll()
        {
            try
            {
                return await _context.LearningCenters
                    .OrderBy(c => c.ShortName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des centres d'apprentissage");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir un centre d'apprentissage par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<LearningCenter>> GetById(int id)
        {
            try
            {
                var center = await _context.LearningCenters.FindAsync(id);
                if (center == null)
                {
                    return NotFound();
                }
                return center;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du centre d'apprentissage {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir un centre d'apprentissage par son code
        /// </summary>
        [HttpGet("code/{code}")]
        public async Task<ActionResult<LearningCenter>> GetByCode(string code)
        {
            try
            {
                var center = await _context.LearningCenters
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
        public async Task<ActionResult<IEnumerable<LearningCenter>>> GetByOrganisation(int organisationId)
        {
            try
            {
                return await _context.LearningCenters
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
        /// Rechercher des centres d'apprentissage
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<LearningCenter>>> Search(
            [FromQuery] string q = null,
            [FromQuery] string city = null,
            [FromQuery] int? province = null,
            [FromQuery] int? region = null)
        {
            try
            {
                var query = _context.LearningCenters.AsQueryable();

                if (!string.IsNullOrWhiteSpace(q))
                {
                    query = query.Where(c =>
                        c.ShortName.Contains(q) ||
                        c.OfficialName.Contains(q) ||
                        c.Code.Contains(q));
                }

                if (!string.IsNullOrWhiteSpace(city))
                {
                    query = query.Where(c => c.City.Contains(city));
                }

                if (province.HasValue)
                {
                    query = query.Where(c => c.Province == province);
                }

                if (region.HasValue)
                {
                    query = query.Where(c => c.AdministrativeRegion == region);
                }

                return await query.OrderBy(c => c.ShortName).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la recherche de centres d'apprentissage");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Créer un nouveau centre d'apprentissage
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<LearningCenter>> Create(LearningCenter center)
        {
            try
            {
                _context.LearningCenters.Add(center);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = center.Id }, center);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création du centre d'apprentissage");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour un centre d'apprentissage
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, LearningCenter center)
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
                _logger.LogError(ex, "Erreur lors de la mise à jour du centre d'apprentissage {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer un centre d'apprentissage
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var center = await _context.LearningCenters.FindAsync(id);
                if (center == null)
                {
                    return NotFound();
                }

                _context.LearningCenters.Remove(center);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression du centre d'apprentissage {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task<bool> CenterExists(int id)
        {
            return await _context.LearningCenters.AnyAsync(e => e.Id == id);
        }
    }
}
