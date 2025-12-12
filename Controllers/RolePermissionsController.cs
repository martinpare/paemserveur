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
    public class RolePermissionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RolePermissionsController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtient toutes les associations rôle-permission
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RolePermission>>> GetRolePermissions()
        {
            return await _context.RolePermissions
                .Include(rp => rp.Role)
                .Include(rp => rp.Permission)
                .ToListAsync();
        }

        /// <summary>
        /// Obtient les permissions d'un rôle (données de base)
        /// </summary>
        [HttpGet("role/{roleId}")]
        public async Task<ActionResult<IEnumerable<RolePermission>>> GetPermissionsByRole(long roleId)
        {
            return await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Include(rp => rp.Permission)
                .ToListAsync();
        }

        /// <summary>
        /// Obtient les permissions complètes d'un rôle (via sp_obtenir_permissions_role)
        /// </summary>
        [HttpGet("role/{roleId}/complet")]
        public async Task<ActionResult<IEnumerable<PermissionRoleInfo>>> GetPermissionsCompletesByRole(long roleId)
        {
            var connection = _context.Database.GetDbConnection();

            var permissions = await connection.QueryAsync<PermissionRoleInfo>(
                "sp_obtenir_permissions_role",
                new { role_id = roleId },
                commandType: CommandType.StoredProcedure);

            return Ok(permissions);
        }

        /// <summary>
        /// Obtient les rôles ayant une permission
        /// </summary>
        [HttpGet("permission/{permissionId}")]
        public async Task<ActionResult<IEnumerable<RolePermission>>> GetRolesByPermission(long permissionId)
        {
            return await _context.RolePermissions
                .Where(rp => rp.PermissionId == permissionId)
                .Include(rp => rp.Role)
                .ToListAsync();
        }

        /// <summary>
        /// Assigne une permission à un rôle (via sp_assigner_permission_role)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<RolePermission>> PostRolePermission(RolePermission rolePermission)
        {
            var connection = _context.Database.GetDbConnection();

            await connection.ExecuteAsync(
                "sp_assigner_permission_role",
                new { permission_id = rolePermission.PermissionId, role_id = rolePermission.RoleId },
                commandType: CommandType.StoredProcedure);

            // Recharger l'association créée
            var created = await _context.RolePermissions
                .Include(rp => rp.Permission)
                .FirstOrDefaultAsync(rp => rp.RoleId == rolePermission.RoleId
                    && rp.PermissionId == rolePermission.PermissionId);

            return CreatedAtAction(nameof(GetPermissionsByRole),
                new { roleId = rolePermission.RoleId }, created);
        }

        /// <summary>
        /// Copie les permissions d'un rôle vers un autre (via sp_copier_permissions_role)
        /// </summary>
        [HttpPost("copier/{roleSourceId}/{roleDestinationId}")]
        public async Task<ActionResult<object>> CopierPermissions(long roleSourceId, long roleDestinationId)
        {
            var connection = _context.Database.GetDbConnection();

            var result = await connection.QueryFirstOrDefaultAsync<CopierPermissionsResult>(
                "sp_copier_permissions_role",
                new { role_source_id = roleSourceId, role_destination_id = roleDestinationId },
                commandType: CommandType.StoredProcedure);

            return Ok(result);
        }

        /// <summary>
        /// Retire une permission d'un rôle (via sp_retirer_permission_role)
        /// </summary>
        [HttpDelete("{roleId}/{permissionId}")]
        public async Task<IActionResult> DeleteRolePermission(long roleId, long permissionId)
        {
            var connection = _context.Database.GetDbConnection();

            var result = await connection.QueryFirstOrDefaultAsync<DeleteResult>(
                "sp_retirer_permission_role",
                new { permission_id = permissionId, role_id = roleId },
                commandType: CommandType.StoredProcedure);

            if (result != null && result.LignesSupprimees == 0)
            {
                return NotFound();
            }

            return NoContent();
        }
    }

    public class CopierPermissionsResult
    {
        public int PermissionsCopiees { get; set; }
    }

    public class DeleteResult
    {
        public int LignesSupprimees { get; set; }
    }
}
