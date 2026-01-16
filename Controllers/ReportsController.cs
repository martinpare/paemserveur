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
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(AppDbContext context, ILogger<ReportsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir tous les rapports
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Report>>> GetAll()
        {
            try
            {
                return await _context.Reports
                    .OrderBy(r => r.SortOrder)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des rapports");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir un rapport par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Report>> GetById(int id)
        {
            try
            {
                var report = await _context.Reports.FindAsync(id);
                if (report == null)
                {
                    return NotFound();
                }
                return report;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du rapport {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir un rapport par son code
        /// </summary>
        [HttpGet("code/{code}")]
        public async Task<ActionResult<Report>> GetByCode(string code)
        {
            try
            {
                var report = await _context.Reports
                    .FirstOrDefaultAsync(r => r.Code == code);
                if (report == null)
                {
                    return NotFound();
                }
                return report;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du rapport par code {Code}", code);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les rapports d'une organisation
        /// </summary>
        [HttpGet("organisation/{organisationId}")]
        public async Task<ActionResult<IEnumerable<Report>>> GetByOrganisation(int organisationId)
        {
            try
            {
                return await _context.Reports
                    .Where(r => r.OrganisationId == organisationId)
                    .OrderBy(r => r.SortOrder)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des rapports de l'organisation {OrganisationId}", organisationId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les rapports d'une structure pédagogique
        /// </summary>
        [HttpGet("pedagogicalstructure/{structureId}")]
        public async Task<ActionResult<IEnumerable<Report>>> GetByStructure(int structureId)
        {
            try
            {
                return await _context.Reports
                    .Where(r => r.PedagogicalStructureId == structureId)
                    .OrderBy(r => r.SortOrder)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des rapports de la structure {StructureId}", structureId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les rapports actifs
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Report>>> GetActive()
        {
            try
            {
                return await _context.Reports
                    .Where(r => r.IsActive)
                    .OrderBy(r => r.SortOrder)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des rapports actifs");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Créer un nouveau rapport
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Report>> Create(Report report)
        {
            try
            {
                _context.Reports.Add(report);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = report.Id }, report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création du rapport");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour un rapport
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Report report)
        {
            if (id != report.Id)
            {
                return BadRequest("L'ID ne correspond pas");
            }

            try
            {
                _context.Entry(report).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ReportExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour du rapport {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer un rapport
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var report = await _context.Reports.FindAsync(id);
                if (report == null)
                {
                    return NotFound();
                }

                _context.Reports.Remove(report);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression du rapport {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task<bool> ReportExists(int id)
        {
            return await _context.Reports.AnyAsync(e => e.Id == id);
        }
    }
}
