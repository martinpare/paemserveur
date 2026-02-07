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
    public class ExamsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ExamsController> _logger;

        public ExamsController(AppDbContext context, ILogger<ExamsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir tous les examens (épreuves principales uniquement, sans les parties)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Exam>>> GetAll()
        {
            try
            {
                return await _context.Exams
                    .Include(e => e.PedagogicalStructure)
                    .Include(e => e.ExamPeriod)
                    .Include(e => e.Competency)
                    .Include(e => e.Language)
                    .Where(e => e.ParentExamId == null) // Épreuves principales uniquement
                    .OrderByDescending(e => e.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des examens");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir un examen par son ID (avec ses parties si c'est une épreuve principale)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Exam>> GetById(int id)
        {
            try
            {
                var exam = await _context.Exams
                    .Include(e => e.PedagogicalStructure)
                    .Include(e => e.ExamPeriod)
                    .Include(e => e.Competency)
                    .Include(e => e.Language)
                    .Include(e => e.ParentExam)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (exam == null)
                {
                    return NotFound();
                }
                return exam;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'examen {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les parties d'une épreuve principale
        /// </summary>
        [HttpGet("{id}/parts")]
        public async Task<ActionResult<IEnumerable<Exam>>> GetParts(int id)
        {
            try
            {
                var parts = await _context.Exams
                    .Include(e => e.Competency)
                    .Where(e => e.ParentExamId == id)
                    .OrderBy(e => e.PartNumber)
                    .ToListAsync();

                return parts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des parties de l'examen {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les examens par structure pédagogique (épreuves principales uniquement)
        /// </summary>
        [HttpGet("pedagogicalstructure/{structureId}")]
        public async Task<ActionResult<IEnumerable<Exam>>> GetByPedagogicalStructure(int structureId)
        {
            try
            {
                return await _context.Exams
                    .Include(e => e.ExamPeriod)
                    .Include(e => e.Competency)
                    .Include(e => e.Language)
                    .Where(e => e.PedagogicalStructureId == structureId && e.ParentExamId == null)
                    .OrderByDescending(e => e.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des examens de la structure {StructureId}", structureId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les examens par période ministérielle (épreuves principales uniquement)
        /// </summary>
        [HttpGet("period/{periodId}")]
        public async Task<ActionResult<IEnumerable<Exam>>> GetByPeriod(int periodId)
        {
            try
            {
                return await _context.Exams
                    .Include(e => e.PedagogicalStructure)
                    .Include(e => e.Competency)
                    .Include(e => e.Language)
                    .Where(e => e.ExamPeriodId == periodId && e.ParentExamId == null)
                    .OrderBy(e => e.ExternalCode)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des examens de la période {PeriodId}", periodId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Créer un nouvel examen
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Exam>> Create(Exam exam)
        {
            try
            {
                exam.CreatedAt = DateTime.UtcNow;
                exam.UpdatedAt = DateTime.UtcNow;

                _context.Exams.Add(exam);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = exam.Id }, exam);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de l'examen");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour un examen
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Exam exam)
        {
            if (id != exam.Id)
            {
                return BadRequest("L'ID ne correspond pas");
            }

            try
            {
                exam.UpdatedAt = DateTime.UtcNow;
                _context.Entry(exam).State = EntityState.Modified;
                _context.Entry(exam).Property(x => x.CreatedAt).IsModified = false;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ExamExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de l'examen {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer un examen
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var exam = await _context.Exams.FindAsync(id);
                if (exam == null)
                {
                    return NotFound();
                }

                // Supprimer les sessions associées
                var sessions = await _context.Sessions.Where(s => s.ExamId == id).ToListAsync();
                if (sessions.Any())
                {
                    _context.Sessions.RemoveRange(sessions);
                }

                _context.Exams.Remove(exam);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de l'examen {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task<bool> ExamExists(int id)
        {
            return await _context.Exams.AnyAsync(e => e.Id == id);
        }

        /// <summary>
        /// Obtenir les examens avec leurs sessions pour une année (vue calendrier)
        /// </summary>
        [HttpGet("calendar/{year}")]
        public async Task<ActionResult<IEnumerable<object>>> GetCalendar(int year)
        {
            try
            {
                var startOfYear = new DateTime(year, 1, 1);
                var endOfYear = new DateTime(year, 12, 31, 23, 59, 59);

                var sessions = await _context.Sessions
                    .Include(s => s.Exam)
                        .ThenInclude(e => e.PedagogicalStructure)
                    .Include(s => s.Exam)
                        .ThenInclude(e => e.ExamPeriod)
                    .Include(s => s.Exam)
                        .ThenInclude(e => e.Competency)
                    .Where(s => s.ScheduledFrom >= startOfYear && s.ScheduledTo <= endOfYear)
                    .OrderBy(s => s.ScheduledFrom)
                    .ToListAsync();

                var result = sessions.Select(s => new
                {
                    sessionId = s.Id,
                    sessionNameFr = s.NameFr,
                    sessionNameEn = s.NameEn,
                    scheduledFrom = s.ScheduledFrom,
                    scheduledTo = s.ScheduledTo,
                    timeStart = s.TimeStart,
                    timeEnd = s.TimeEnd,
                    exam = new
                    {
                        id = s.Exam.Id,
                        nameFr = s.Exam.NameFr,
                        nameEn = s.Exam.NameEn,
                        externalCode = s.Exam.ExternalCode,
                        pedagogicalStructureId = s.Exam.PedagogicalStructureId,
                        pedagogicalStructureName = s.Exam.PedagogicalStructure?.NameFr,
                        examPeriodId = s.Exam.ExamPeriodId,
                        examPeriodName = s.Exam.ExamPeriod?.NameFr,
                        competencyId = s.Exam.CompetencyId,
                        competencyName = s.Exam.Competency?.ValueFr
                    }
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du calendrier des examens pour l'année {Year}", year);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }
    }
}
