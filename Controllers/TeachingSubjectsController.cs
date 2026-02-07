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
    public class TeachingSubjectsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TeachingSubjectsController> _logger;

        public TeachingSubjectsController(AppDbContext context, ILogger<TeachingSubjectsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir toutes les matières d'enseignement
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeachingSubject>>> GetAll([FromQuery] bool? activeOnly = null)
        {
            try
            {
                var query = _context.TeachingSubjects.AsQueryable();

                if (activeOnly == true)
                {
                    query = query.Where(t => t.IsActive);
                }

                return await query.OrderBy(t => t.NameFr).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des matières d'enseignement");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir une matière d'enseignement par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TeachingSubject>> GetById(int id)
        {
            try
            {
                var subject = await _context.TeachingSubjects.FindAsync(id);

                if (subject == null)
                {
                    return NotFound();
                }

                return subject;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de la matière {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Créer une nouvelle matière d'enseignement
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TeachingSubject>> Create(TeachingSubject subject)
        {
            try
            {
                subject.CreatedAt = DateTime.UtcNow;
                subject.UpdatedAt = DateTime.UtcNow;

                _context.TeachingSubjects.Add(subject);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = subject.Id }, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de la matière");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour une matière d'enseignement
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TeachingSubject subject)
        {
            if (id != subject.Id)
            {
                return BadRequest("L'ID ne correspond pas");
            }

            try
            {
                subject.UpdatedAt = DateTime.UtcNow;
                _context.Entry(subject).State = EntityState.Modified;
                _context.Entry(subject).Property(x => x.CreatedAt).IsModified = false;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await SubjectExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de la matière {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer une matière d'enseignement
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var subject = await _context.TeachingSubjects.FindAsync(id);
                if (subject == null)
                {
                    return NotFound();
                }

                _context.TeachingSubjects.Remove(subject);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de la matière {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task<bool> SubjectExists(int id)
        {
            return await _context.TeachingSubjects.AnyAsync(t => t.Id == id);
        }
    }
}
