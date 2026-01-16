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
    public class OrganisationsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<OrganisationsController> _logger;

        public OrganisationsController(AppDbContext context, ILogger<OrganisationsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir toutes les organisations
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Organisation>>> GetAll()
        {
            try
            {
                return await _context.Organisations.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des organisations");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir une organisation par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Organisation>> GetById(int id)
        {
            try
            {
                var organisation = await _context.Organisations.FindAsync(id);
                if (organisation == null)
                {
                    return NotFound();
                }
                return organisation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'organisation {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les organisations actives
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Organisation>>> GetActive()
        {
            try
            {
                return await _context.Organisations
                    .Where(o => o.IsActive)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des organisations actives");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Créer une nouvelle organisation
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Organisation>> Create(Organisation organisation)
        {
            try
            {
                _context.Organisations.Add(organisation);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = organisation.Id }, organisation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de l'organisation");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour une organisation
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Organisation organisation)
        {
            if (id != organisation.Id)
            {
                return BadRequest("L'ID ne correspond pas");
            }

            try
            {
                _context.Entry(organisation).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await OrganisationExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de l'organisation {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer une organisation
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var organisation = await _context.Organisations.FindAsync(id);
                if (organisation == null)
                {
                    return NotFound();
                }

                _context.Organisations.Remove(organisation);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de l'organisation {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task<bool> OrganisationExists(int id)
        {
            return await _context.Organisations.AnyAsync(e => e.Id == id);
        }
    }
}
