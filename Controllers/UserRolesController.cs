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
    public class UserRolesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserRolesController> _logger;

        public UserRolesController(AppDbContext context, ILogger<UserRolesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir toutes les assignations utilisateur-rôle
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserRole>>> GetAll()
        {
            try
            {
                return await _context.UserRoles.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des assignations utilisateur-rôle");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir une assignation par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserRole>> GetById(int id)
        {
            try
            {
                var userRole = await _context.UserRoles.FindAsync(id);
                if (userRole == null)
                {
                    return NotFound();
                }
                return userRole;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'assignation {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les rôles d'un utilisateur
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<UserRole>>> GetByUser(int userId)
        {
            try
            {
                return await _context.UserRoles
                    .Where(ur => ur.UserId == userId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des rôles de l'utilisateur {UserId}", userId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les utilisateurs ayant un rôle
        /// </summary>
        [HttpGet("role/{roleId}")]
        public async Task<ActionResult<IEnumerable<UserRole>>> GetByRole(int roleId)
        {
            try
            {
                return await _context.UserRoles
                    .Where(ur => ur.RoleId == roleId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des utilisateurs du rôle {RoleId}", roleId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Assigner un rôle à un utilisateur
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<UserRole>> Create(UserRole userRole)
        {
            try
            {
                // Vérifier si l'assignation existe déjà
                var exists = await _context.UserRoles
                    .AnyAsync(ur => ur.UserId == userRole.UserId &&
                                    ur.RoleId == userRole.RoleId &&
                                    ur.OrganisationId == userRole.OrganisationId);

                if (exists)
                {
                    return BadRequest("Cet utilisateur a déjà ce rôle assigné");
                }

                _context.UserRoles.Add(userRole);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = userRole.Id }, userRole);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'assignation du rôle");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer une assignation par son ID
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userRole = await _context.UserRoles.FindAsync(id);
                if (userRole == null)
                {
                    return NotFound();
                }

                _context.UserRoles.Remove(userRole);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de l'assignation {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer une assignation par utilisateur et rôle
        /// </summary>
        [HttpDelete("user/{userId}/role/{roleId}")]
        public async Task<IActionResult> DeleteByComposite(int userId, int roleId, [FromQuery] int? organisationId = null)
        {
            try
            {
                var userRole = await _context.UserRoles
                    .FirstOrDefaultAsync(ur => ur.UserId == userId &&
                                               ur.RoleId == roleId &&
                                               ur.OrganisationId == organisationId);

                if (userRole == null)
                {
                    return NotFound();
                }

                _context.UserRoles.Remove(userRole);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de l'assignation utilisateur {UserId} / rôle {RoleId}", userId, roleId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }
    }
}
