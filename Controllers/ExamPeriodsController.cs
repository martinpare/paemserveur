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
    public class ExamPeriodsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ExamPeriodsController> _logger;

        public ExamPeriodsController(AppDbContext context, ILogger<ExamPeriodsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir toutes les périodes d'examen
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExamPeriod>>> GetAll()
        {
            try
            {
                return await _context.ExamPeriods
                    .OrderByDescending(ep => ep.Year)
                    .ThenByDescending(ep => ep.Month)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des périodes d'examen");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir une période d'examen par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ExamPeriod>> GetById(int id)
        {
            try
            {
                var examPeriod = await _context.ExamPeriods.FindAsync(id);

                if (examPeriod == null)
                {
                    return NotFound();
                }
                return examPeriod;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de la période d'examen {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les périodes d'examen actives
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<ExamPeriod>>> GetActive()
        {
            try
            {
                return await _context.ExamPeriods
                    .Where(ep => ep.IsActive)
                    .OrderByDescending(ep => ep.Year)
                    .ThenByDescending(ep => ep.Month)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des périodes d'examen actives");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les périodes d'examen par année
        /// </summary>
        [HttpGet("year/{year}")]
        public async Task<ActionResult<IEnumerable<ExamPeriod>>> GetByYear(int year)
        {
            try
            {
                return await _context.ExamPeriods
                    .Where(ep => ep.Year == year)
                    .OrderBy(ep => ep.Month)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des périodes d'examen pour l'année {Year}", year);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Créer une nouvelle période d'examen
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ExamPeriod>> Create(ExamPeriod examPeriod)
        {
            try
            {
                examPeriod.CreatedAt = DateTime.UtcNow;
                examPeriod.UpdatedAt = DateTime.UtcNow;

                _context.ExamPeriods.Add(examPeriod);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = examPeriod.Id }, examPeriod);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de la période d'examen");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour une période d'examen
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ExamPeriod examPeriod)
        {
            if (id != examPeriod.Id)
            {
                return BadRequest("L'ID ne correspond pas");
            }

            try
            {
                examPeriod.UpdatedAt = DateTime.UtcNow;
                _context.Entry(examPeriod).State = EntityState.Modified;
                _context.Entry(examPeriod).Property(x => x.CreatedAt).IsModified = false;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ExamPeriodExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de la période d'examen {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer une période d'examen
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var examPeriod = await _context.ExamPeriods.FindAsync(id);
                if (examPeriod == null)
                {
                    return NotFound();
                }

                _context.ExamPeriods.Remove(examPeriod);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de la période d'examen {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task<bool> ExamPeriodExists(int id)
        {
            return await _context.ExamPeriods.AnyAsync(ep => ep.Id == id);
        }
    }
}
