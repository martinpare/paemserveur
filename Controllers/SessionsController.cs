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
    public class SessionsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SessionsController> _logger;

        public SessionsController(AppDbContext context, ILogger<SessionsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir toutes les sessions
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Session>>> GetAll()
        {
            try
            {
                return await _context.Sessions
                    .OrderByDescending(s => s.ScheduledAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des sessions");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir une session par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Session>> GetById(int id)
        {
            try
            {
                var session = await _context.Sessions.FindAsync(id);
                if (session == null)
                {
                    return NotFound();
                }
                return session;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de la session {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les sessions par structure pédagogique
        /// </summary>
        [HttpGet("pedagogicalstructure/{structureId}")]
        public async Task<ActionResult<IEnumerable<Session>>> GetByPedagogicalStructure(int structureId)
        {
            try
            {
                return await _context.Sessions
                    .Where(s => s.PedagogicalStructureId == structureId)
                    .OrderByDescending(s => s.ScheduledAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des sessions de la structure pédagogique {StructureId}", structureId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les sessions par test
        /// </summary>
        [HttpGet("test/{testId}")]
        public async Task<ActionResult<IEnumerable<Session>>> GetByTest(int testId)
        {
            try
            {
                return await _context.Sessions
                    .Where(s => s.TestId == testId)
                    .OrderByDescending(s => s.ScheduledAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des sessions du test {TestId}", testId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Créer une nouvelle session
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Session>> Create(Session session)
        {
            try
            {
                _context.Sessions.Add(session);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = session.Id }, session);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de la session");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour une session
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Session session)
        {
            if (id != session.Id)
            {
                return BadRequest("L'ID ne correspond pas");
            }

            try
            {
                _context.Entry(session).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await SessionExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de la session {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer une session
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var session = await _context.Sessions.FindAsync(id);
                if (session == null)
                {
                    return NotFound();
                }

                _context.Sessions.Remove(session);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de la session {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task<bool> SessionExists(int id)
        {
            return await _context.Sessions.AnyAsync(e => e.Id == id);
        }
    }
}
