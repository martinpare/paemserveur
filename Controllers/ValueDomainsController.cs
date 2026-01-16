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
    public class ValueDomainsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ValueDomainsController> _logger;

        public ValueDomainsController(AppDbContext context, ILogger<ValueDomainsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir tous les domaines de valeurs
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ValueDomain>>> GetAll()
        {
            try
            {
                return await _context.ValueDomains.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des domaines de valeurs");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir un domaine de valeurs par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ValueDomain>> GetById(int id)
        {
            try
            {
                var valueDomain = await _context.ValueDomains.FindAsync(id);
                if (valueDomain == null)
                {
                    return NotFound();
                }
                return valueDomain;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du domaine de valeurs {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir un domaine de valeurs par son tag
        /// </summary>
        [HttpGet("tag/{tag}")]
        public async Task<ActionResult<ValueDomain>> GetByTag(string tag)
        {
            try
            {
                var valueDomain = await _context.ValueDomains
                    .FirstOrDefaultAsync(v => v.Tag == tag);
                if (valueDomain == null)
                {
                    return NotFound();
                }
                return valueDomain;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du domaine de valeurs par tag {Tag}", tag);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les domaines de valeurs publics
        /// </summary>
        [HttpGet("public")]
        public async Task<ActionResult<IEnumerable<ValueDomain>>> GetPublic()
        {
            try
            {
                return await _context.ValueDomains
                    .Where(v => v.IsPublic)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des domaines de valeurs publics");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir un domaine de valeurs avec ses items
        /// </summary>
        [HttpGet("{id}/items")]
        public async Task<ActionResult<object>> GetWithItems(int id)
        {
            try
            {
                var valueDomain = await _context.ValueDomains.FindAsync(id);
                if (valueDomain == null)
                {
                    return NotFound();
                }

                var items = await _context.ValueDomainItems
                    .Where(i => i.ValueDomainId == id)
                    .OrderBy(i => i.Order)
                    .ToListAsync();

                return new { Domain = valueDomain, Items = items };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du domaine avec items {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Créer un nouveau domaine de valeurs
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ValueDomain>> Create(ValueDomain valueDomain)
        {
            try
            {
                // Vérifier si le tag existe déjà
                if (await _context.ValueDomains.AnyAsync(v => v.Tag == valueDomain.Tag))
                {
                    return BadRequest("Un domaine avec ce tag existe déjà");
                }

                _context.ValueDomains.Add(valueDomain);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = valueDomain.Id }, valueDomain);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création du domaine de valeurs");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour un domaine de valeurs
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ValueDomain valueDomain)
        {
            if (id != valueDomain.Id)
            {
                return BadRequest("L'ID ne correspond pas");
            }

            try
            {
                // Vérifier si le tag existe déjà pour un autre domaine
                if (await _context.ValueDomains.AnyAsync(v => v.Tag == valueDomain.Tag && v.Id != id))
                {
                    return BadRequest("Un autre domaine avec ce tag existe déjà");
                }

                _context.Entry(valueDomain).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ValueDomainExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour du domaine de valeurs {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer un domaine de valeurs
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var valueDomain = await _context.ValueDomains.FindAsync(id);
                if (valueDomain == null)
                {
                    return NotFound();
                }

                // Vérifier si le domaine a des items
                var hasItems = await _context.ValueDomainItems.AnyAsync(i => i.ValueDomainId == id);
                if (hasItems)
                {
                    return BadRequest("Impossible de supprimer un domaine contenant des items");
                }

                _context.ValueDomains.Remove(valueDomain);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression du domaine de valeurs {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task<bool> ValueDomainExists(int id)
        {
            return await _context.ValueDomains.AnyAsync(e => e.Id == id);
        }
    }
}
