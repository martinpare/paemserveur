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
    public class ConvocationsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ConvocationsController> _logger;

        public ConvocationsController(AppDbContext context, ILogger<ConvocationsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir toutes les convocations
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Convocation>>> GetAll()
        {
            try
            {
                return await _context.Convocations
                    .Include(c => c.Exam)
                    .Include(c => c.Learner)
                    .Include(c => c.Session)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des convocations");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir une convocation par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Convocation>> GetById(int id)
        {
            try
            {
                var convocation = await _context.Convocations
                    .Include(c => c.Exam)
                    .Include(c => c.Learner)
                    .Include(c => c.Session)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (convocation == null)
                {
                    return NotFound();
                }
                return convocation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de la convocation {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les convocations par examen
        /// </summary>
        [HttpGet("by-exam/{examId}")]
        public async Task<ActionResult<IEnumerable<Convocation>>> GetByExam(int examId)
        {
            try
            {
                return await _context.Convocations
                    .Include(c => c.Learner)
                    .Include(c => c.Session)
                    .Where(c => c.ExamId == examId)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des convocations de l'examen {ExamId}", examId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les convocations par session
        /// </summary>
        [HttpGet("by-session/{sessionId}")]
        public async Task<ActionResult<IEnumerable<Convocation>>> GetBySession(int sessionId)
        {
            try
            {
                return await _context.Convocations
                    .Include(c => c.Exam)
                    .Include(c => c.Learner)
                    .Where(c => c.SessionId == sessionId)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des convocations de la session {SessionId}", sessionId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les convocations par apprenant
        /// </summary>
        [HttpGet("by-learner/{learnerId}")]
        public async Task<ActionResult<IEnumerable<Convocation>>> GetByLearner(int learnerId)
        {
            try
            {
                return await _context.Convocations
                    .Include(c => c.Exam)
                    .Include(c => c.Session)
                    .Where(c => c.LearnerId == learnerId)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des convocations de l'apprenant {LearnerId}", learnerId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Créer une convocation
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Convocation>> Create(Convocation convocation)
        {
            try
            {
                convocation.CreatedAt = DateTime.UtcNow;
                convocation.UpdatedAt = DateTime.UtcNow;

                _context.Convocations.Add(convocation);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = convocation.Id }, convocation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de la convocation");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour une convocation
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Convocation convocation)
        {
            if (id != convocation.Id)
            {
                return BadRequest("L'ID ne correspond pas");
            }

            try
            {
                convocation.UpdatedAt = DateTime.UtcNow;
                _context.Entry(convocation).State = EntityState.Modified;
                _context.Entry(convocation).Property(x => x.CreatedAt).IsModified = false;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ConvocationExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de la convocation {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer une convocation
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var convocation = await _context.Convocations.FindAsync(id);
                if (convocation == null)
                {
                    return NotFound();
                }

                _context.Convocations.Remove(convocation);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de la convocation {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task<bool> ConvocationExists(int id)
        {
            return await _context.Convocations.AnyAsync(c => c.Id == id);
        }
    }
}
