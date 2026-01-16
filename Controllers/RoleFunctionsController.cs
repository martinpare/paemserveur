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
    public class RoleFunctionsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<RoleFunctionsController> _logger;

        public RoleFunctionsController(AppDbContext context, ILogger<RoleFunctionsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir toutes les assignations rôle-fonction
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleFunction>>> GetAll()
        {
            try
            {
                return await _context.RoleFunctions.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des assignations rôle-fonction");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir une assignation par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleFunction>> GetById(int id)
        {
            try
            {
                var roleFunction = await _context.RoleFunctions.FindAsync(id);
                if (roleFunction == null)
                {
                    return NotFound();
                }
                return roleFunction;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'assignation {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les fonctions d'un rôle
        /// </summary>
        [HttpGet("role/{roleId}")]
        public async Task<ActionResult<IEnumerable<RoleFunction>>> GetByRole(int roleId)
        {
            try
            {
                return await _context.RoleFunctions
                    .Where(rf => rf.RoleId == roleId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des fonctions du rôle {RoleId}", roleId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les rôles ayant une fonction
        /// </summary>
        [HttpGet("function/{functionCode}")]
        public async Task<ActionResult<IEnumerable<RoleFunction>>> GetByFunction(string functionCode)
        {
            try
            {
                return await _context.RoleFunctions
                    .Where(rf => rf.FunctionCode == functionCode)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des rôles de la fonction {FunctionCode}", functionCode);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Assigner une fonction à un rôle
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<RoleFunction>> Create(RoleFunction roleFunction)
        {
            try
            {
                // Vérifier si l'assignation existe déjà
                var exists = await _context.RoleFunctions
                    .AnyAsync(rf => rf.RoleId == roleFunction.RoleId &&
                                    rf.FunctionCode == roleFunction.FunctionCode);

                if (exists)
                {
                    return BadRequest("Ce rôle a déjà cette fonction assignée");
                }

                _context.RoleFunctions.Add(roleFunction);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = roleFunction.Id }, roleFunction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'assignation de la fonction");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Assigner plusieurs fonctions à un rôle
        /// </summary>
        [HttpPost("role/{roleId}/bulk")]
        public async Task<IActionResult> BulkAssign(int roleId, [FromBody] List<string> functionCodes)
        {
            try
            {
                // Supprimer les anciennes assignations
                var existingAssignments = await _context.RoleFunctions
                    .Where(rf => rf.RoleId == roleId)
                    .ToListAsync();
                _context.RoleFunctions.RemoveRange(existingAssignments);

                // Créer les nouvelles assignations
                var newAssignments = functionCodes.Select(code => new RoleFunction
                {
                    RoleId = roleId,
                    FunctionCode = code
                });
                _context.RoleFunctions.AddRange(newAssignments);

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'assignation en lot des fonctions au rôle {RoleId}", roleId);
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
                var roleFunction = await _context.RoleFunctions.FindAsync(id);
                if (roleFunction == null)
                {
                    return NotFound();
                }

                _context.RoleFunctions.Remove(roleFunction);
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
        /// Supprimer une assignation par rôle et fonction
        /// </summary>
        [HttpDelete("role/{roleId}/function/{functionCode}")]
        public async Task<IActionResult> DeleteByComposite(int roleId, string functionCode)
        {
            try
            {
                var roleFunction = await _context.RoleFunctions
                    .FirstOrDefaultAsync(rf => rf.RoleId == roleId && rf.FunctionCode == functionCode);

                if (roleFunction == null)
                {
                    return NotFound();
                }

                _context.RoleFunctions.Remove(roleFunction);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de l'assignation rôle {RoleId} / fonction {FunctionCode}", roleId, functionCode);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }
    }
}
