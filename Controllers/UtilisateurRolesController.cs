using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dapper;
using serveur.Models;

namespace serveur.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UtilisateurRolesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UtilisateurRolesController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtient toutes les associations utilisateur-rôle
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UtilisateurRole>>> GetUtilisateurRoles()
        {
            return await _context.UtilisateurRoles
                .Include(ur => ur.Utilisateur)
                .Include(ur => ur.Role)
                .ToListAsync();
        }

        /// <summary>
        /// Obtient les rôles d'un utilisateur (données de base)
        /// </summary>
        [HttpGet("utilisateur/{utilisateurId}")]
        public async Task<ActionResult<IEnumerable<UtilisateurRole>>> GetRolesByUtilisateur(long utilisateurId)
        {
            return await _context.UtilisateurRoles
                .Where(ur => ur.UtilisateurId == utilisateurId)
                .Include(ur => ur.Role)
                .ToListAsync();
        }

        /// <summary>
        /// Obtient tous les rôles d'un utilisateur (globaux et avec portée) via sp_obtenir_roles_utilisateur
        /// </summary>
        [HttpGet("utilisateur/{utilisateurId}/complet")]
        public async Task<ActionResult<IEnumerable<RoleUtilisateurInfo>>> GetRolesCompletsByUtilisateur(long utilisateurId)
        {
            var connection = _context.Database.GetDbConnection();

            var roles = await connection.QueryAsync<RoleUtilisateurInfo>(
                "sp_obtenir_roles_utilisateur",
                new { utilisateur_id = utilisateurId },
                commandType: CommandType.StoredProcedure);

            return Ok(roles);
        }

        /// <summary>
        /// Obtient les utilisateurs ayant un rôle spécifique
        /// </summary>
        [HttpGet("role/{roleId}")]
        public async Task<ActionResult<IEnumerable<UtilisateurRole>>> GetUtilisateursByRole(long roleId)
        {
            return await _context.UtilisateurRoles
                .Where(ur => ur.RoleId == roleId)
                .Include(ur => ur.Utilisateur)
                .ToListAsync();
        }

        /// <summary>
        /// Assigne un rôle à un utilisateur (via sp_assigner_role_utilisateur)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<UtilisateurRole>> PostUtilisateurRole(UtilisateurRole utilisateurRole)
        {
            var connection = _context.Database.GetDbConnection();

            await connection.ExecuteAsync(
                "sp_assigner_role_utilisateur",
                new { utilisateur_id = utilisateurRole.UtilisateurId, role_id = utilisateurRole.RoleId },
                commandType: CommandType.StoredProcedure);

            // Recharger l'association créée
            var created = await _context.UtilisateurRoles
                .Include(ur => ur.Role)
                .FirstOrDefaultAsync(ur => ur.UtilisateurId == utilisateurRole.UtilisateurId
                    && ur.RoleId == utilisateurRole.RoleId);

            return CreatedAtAction(nameof(GetRolesByUtilisateur),
                new { utilisateurId = utilisateurRole.UtilisateurId }, created);
        }

        /// <summary>
        /// Retire un rôle d'un utilisateur (via sp_retirer_role_utilisateur)
        /// </summary>
        [HttpDelete("{utilisateurId}/{roleId}")]
        public async Task<IActionResult> DeleteUtilisateurRole(long utilisateurId, long roleId)
        {
            var connection = _context.Database.GetDbConnection();

            var result = await connection.QueryFirstOrDefaultAsync<DeleteResult>(
                "sp_retirer_role_utilisateur",
                new { utilisateur_id = utilisateurId, role_id = roleId },
                commandType: CommandType.StoredProcedure);

            if (result != null && result.LignesSupprimees == 0)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
