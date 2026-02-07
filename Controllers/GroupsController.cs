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
    public class GroupsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<GroupsController> _logger;

        public GroupsController(AppDbContext context, ILogger<GroupsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir tous les groupes
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Group>>> GetAll()
        {
            try
            {
                return await _context.Groups
                    .Include(g => g.LearningCenter)
                    .Include(g => g.Grade)
                    .Include(g => g.AcademicYear)
                    .Include(g => g.Language)
                    .Include(g => g.Teacher)
                    .Include(g => g.Proctor)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la recuperation des groupes");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir un groupe par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Group>> GetById(int id)
        {
            try
            {
                var group = await _context.Groups
                    .Include(g => g.LearningCenter)
                    .Include(g => g.Grade)
                    .Include(g => g.AcademicYear)
                    .Include(g => g.Language)
                    .Include(g => g.Teacher)
                    .Include(g => g.Proctor)
                    .FirstOrDefaultAsync(g => g.Id == id);

                if (group == null)
                {
                    return NotFound();
                }
                return group;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la recuperation du groupe {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les groupes par centre d'apprentissage
        /// </summary>
        [HttpGet("by-learning-center/{learningCenterId}")]
        public async Task<ActionResult<IEnumerable<Group>>> GetByLearningCenter(int learningCenterId)
        {
            try
            {
                return await _context.Groups
                    .Where(g => g.LearningCenterId == learningCenterId)
                    .Include(g => g.Grade)
                    .Include(g => g.AcademicYear)
                    .Include(g => g.Language)
                    .Include(g => g.Teacher)
                    .Include(g => g.Proctor)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la recuperation des groupes du centre {LearningCenterId}", learningCenterId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les groupes par annee scolaire
        /// </summary>
        [HttpGet("by-academic-year/{academicYearId}")]
        public async Task<ActionResult<IEnumerable<Group>>> GetByAcademicYear(int academicYearId)
        {
            try
            {
                return await _context.Groups
                    .Where(g => g.AcademicYearId == academicYearId)
                    .Include(g => g.LearningCenter)
                    .Include(g => g.Grade)
                    .Include(g => g.Language)
                    .Include(g => g.Teacher)
                    .Include(g => g.Proctor)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la recuperation des groupes de l'annee {AcademicYearId}", academicYearId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Creer un nouveau groupe
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Group>> Create(Group group)
        {
            try
            {
                group.CreatedAt = DateTime.UtcNow;
                group.UpdatedAt = DateTime.UtcNow;

                _context.Groups.Add(group);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = group.Id }, group);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la creation du groupe");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre a jour un groupe
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Group group)
        {
            if (id != group.Id)
            {
                return BadRequest("L'ID ne correspond pas");
            }

            try
            {
                group.UpdatedAt = DateTime.UtcNow;
                _context.Entry(group).State = EntityState.Modified;
                _context.Entry(group).Property(x => x.CreatedAt).IsModified = false;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await GroupExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise a jour du groupe {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer un groupe
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var group = await _context.Groups.FindAsync(id);
                if (group == null)
                {
                    return NotFound();
                }

                _context.Groups.Remove(group);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression du groupe {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task<bool> GroupExists(int id)
        {
            return await _context.Groups.AnyAsync(e => e.Id == id);
        }
    }
}
