using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dapper;
using serveur.Models;
using serveur.Services;

namespace serveur.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PermissionsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPermissionGestionService _permissionGestionService;

        public PermissionsController(AppDbContext context, IPermissionGestionService permissionGestionService)
        {
            _context = context;
            _permissionGestionService = permissionGestionService;
        }

        /// <summary>
        /// Obtient toutes les permissions (données de base avec ressource)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Permission>>> GetPermissions()
        {
            return await _context.Permissions.Include(p => p.Ressource).ToListAsync();
        }

        /// <summary>
        /// Obtient toutes les permissions avec ressources (via v_permission_ressource)
        /// </summary>
        [HttpGet("completes")]
        public async Task<ActionResult<IEnumerable<PermissionRessource>>> GetPermissionsCompletes()
        {
            return await _context.PermissionsRessources.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Obtient l'utilisation des permissions (quels rôles, combien d'utilisateurs)
        /// </summary>
        [HttpGet("usage")]
        public async Task<ActionResult<IEnumerable<PermissionUsage>>> GetPermissionsUsage()
        {
            return await _context.PermissionsUsage.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Obtient les permissions orphelines (non assignées à aucun rôle)
        /// </summary>
        [HttpGet("orphelines")]
        public async Task<ActionResult<IEnumerable<PermissionOrpheline>>> GetPermissionsOrphelines()
        {
            return await _context.PermissionsOrphelines.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Obtient une permission par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Permission>> GetPermission(long id)
        {
            var permission = await _context.Permissions.Include(p => p.Ressource).FirstOrDefaultAsync(p => p.Id == id);
            if (permission == null)
            {
                return NotFound();
            }
            return permission;
        }

        /// <summary>
        /// Obtient une permission complète par son ID (via v_permission_ressource)
        /// </summary>
        [HttpGet("{id}/complete")]
        public async Task<ActionResult<PermissionRessource>> GetPermissionComplete(long id)
        {
            var permission = await _context.PermissionsRessources
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PermissionId == id);

            if (permission == null)
            {
                return NotFound();
            }
            return permission;
        }

        /// <summary>
        /// Obtient l'utilisation d'une permission spécifique
        /// </summary>
        [HttpGet("{id}/usage")]
        public async Task<ActionResult<IEnumerable<PermissionUsage>>> GetPermissionUsage(long id)
        {
            var usage = await _context.PermissionsUsage
                .AsNoTracking()
                .Where(p => p.PermissionId == id)
                .ToListAsync();

            if (!usage.Any())
            {
                return NotFound();
            }
            return usage;
        }

        /// <summary>
        /// Recherche des permissions par action
        /// </summary>
        [HttpGet("par-action/{action}")]
        public async Task<ActionResult<IEnumerable<PermissionRessource>>> GetPermissionsParAction(string action)
        {
            return await _context.PermissionsRessources
                .AsNoTracking()
                .Where(p => p.Action == action)
                .ToListAsync();
        }

        /// <summary>
        /// Recherche des permissions par type de ressource
        /// </summary>
        [HttpGet("par-ressource-type/{type}")]
        public async Task<ActionResult<IEnumerable<PermissionRessource>>> GetPermissionsParRessourceType(string type)
        {
            return await _context.PermissionsRessources
                .AsNoTracking()
                .Where(p => p.RessourceType == type)
                .ToListAsync();
        }

        /// <summary>
        /// Crée une nouvelle permission (via sp_creer_permission)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CreerPermissionResult>> PostPermission([FromBody] CreerPermissionDto dto)
        {
            var connection = _context.Database.GetDbConnection();

            var result = await connection.QueryFirstOrDefaultAsync<CreerPermissionResult>(
                "sp_creer_permission",
                new { code = dto.Code, action = dto.Action, ressource_id = dto.RessourceId, description = dto.Description },
                commandType: CommandType.StoredProcedure);

            if (result != null)
            {
                return CreatedAtAction(nameof(GetPermission), new { id = result.PermissionId }, result);
            }
            return BadRequest("Erreur lors de la création de la permission");
        }

        /// <summary>
        /// Crée les 4 permissions CRUD pour une ressource (via sp_creer_permissions_crud)
        /// </summary>
        [HttpPost("crud/{ressourceId}")]
        public async Task<ActionResult<IEnumerable<CreerPermissionsCrudResult>>> CreerPermissionsCrud(
            long ressourceId,
            [FromQuery] string prefixeCode = null)
        {
            var connection = _context.Database.GetDbConnection();

            var result = await connection.QueryAsync<CreerPermissionsCrudResult>(
                "sp_creer_permissions_crud",
                new { ressource_id = ressourceId, prefixe_code = prefixeCode },
                commandType: CommandType.StoredProcedure);

            return Ok(result);
        }

        /// <summary>
        /// Met à jour une permission
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPermission(long id, Permission permission)
        {
            if (id != permission.Id)
            {
                return BadRequest();
            }

            _context.Entry(permission).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Supprime une permission et ses associations (via sp_supprimer_permission)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePermission(long id)
        {
            var connection = _context.Database.GetDbConnection();

            await connection.ExecuteAsync(
                "sp_supprimer_permission",
                new { permission_id = id },
                commandType: CommandType.StoredProcedure);

            return NoContent();
        }

        #region Endpoints utilisant les nouvelles vues

        /// <summary>
        /// Obtient toutes les permissions avec statistiques complètes (via v_permission_complete)
        /// </summary>
        [HttpGet("avec-stats")]
        public async Task<ActionResult<IEnumerable<PermissionComplete>>> GetPermissionsAvecStats()
        {
            return await _context.PermissionsCompletes.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Obtient les permissions regroupées par action (via v_permission_par_action)
        /// </summary>
        [HttpGet("par-action")]
        public async Task<ActionResult<IEnumerable<PermissionParAction>>> GetPermissionsGroupeesParAction()
        {
            return await _context.PermissionsParAction.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Obtient les permissions avec leurs rôles et utilisateurs (via v_permission_role_utilisateur)
        /// </summary>
        [HttpGet("role-utilisateur")]
        public async Task<ActionResult<IEnumerable<PermissionRoleUtilisateur>>> GetPermissionsRoleUtilisateur()
        {
            return await _context.PermissionsRoleUtilisateurs.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Obtient les permissions d'un rôle spécifique (via v_permission_role_utilisateur)
        /// </summary>
        [HttpGet("role/{roleId}")]
        public async Task<ActionResult<IEnumerable<PermissionRoleUtilisateur>>> GetPermissionsParRole(long roleId)
        {
            return await _context.PermissionsRoleUtilisateurs
                .AsNoTracking()
                .Where(p => p.RoleId == roleId)
                .ToListAsync();
        }

        /// <summary>
        /// Obtient les permissions non liées à une ressource (via v_permission_sans_ressource)
        /// </summary>
        [HttpGet("sans-ressource")]
        public async Task<ActionResult<IEnumerable<PermissionSansRessource>>> GetPermissionsSansRessource()
        {
            return await _context.PermissionsSansRessource.AsNoTracking().ToListAsync();
        }

        #endregion

        #region Endpoints utilisant les procédures stockées via service

        /// <summary>
        /// Crée une permission via service (sp_creer_permission)
        /// </summary>
        [HttpPost("creer")]
        public async Task<ActionResult<CreerPermissionResult>> CreerPermission(CreerPermissionDto dto)
        {
            try
            {
                var result = await _permissionGestionService.CreerPermission(dto);
                if (result == null || result.PermissionId <= 0)
                {
                    return BadRequest("Erreur lors de la création de la permission");
                }
                return CreatedAtAction(nameof(GetPermission), new { id = result.PermissionId }, result);
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Modifie une permission via service (sp_modifier_permission)
        /// </summary>
        [HttpPut("{id}/modifier")]
        public async Task<IActionResult> ModifierPermission(long id, ModifierPermissionDto dto)
        {
            try
            {
                var success = await _permissionGestionService.ModifierPermission(id, dto);
                if (!success)
                {
                    return BadRequest("Erreur lors de la modification de la permission");
                }
                return NoContent();
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Supprime une permission de manière sécurisée via service (sp_supprimer_permission)
        /// </summary>
        [HttpDelete("{id}/supprimer")]
        public async Task<IActionResult> SupprimerPermission(long id, [FromQuery] bool force = false)
        {
            try
            {
                var success = await _permissionGestionService.SupprimerPermission(id, force);
                if (!success)
                {
                    return BadRequest("Erreur lors de la suppression de la permission");
                }
                return NoContent();
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Duplique une permission
        /// </summary>
        [HttpPost("{id}/dupliquer")]
        public async Task<ActionResult<long>> DupliquerPermission(long id, PermissionDuplicationDto dto)
        {
            try
            {
                var nouvelId = await _permissionGestionService.DupliquerPermission(id, dto);
                if (nouvelId <= 0)
                {
                    return BadRequest("Erreur lors de la duplication de la permission");
                }
                return CreatedAtAction(nameof(GetPermission), new { id = nouvelId }, nouvelId);
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Obtient une permission avec toutes ses statistiques (sp_obtenir_permission_complete)
        /// </summary>
        [HttpGet("{id}/stats")]
        public async Task<ActionResult<PermissionAvecStats>> GetPermissionStats(long id)
        {
            var permission = await _permissionGestionService.ObtenirPermissionComplete(id);
            if (permission == null)
            {
                return NotFound();
            }
            return permission;
        }

        /// <summary>
        /// Recherche des permissions avec filtres (sp_rechercher_permissions)
        /// </summary>
        [HttpPost("rechercher")]
        public async Task<ActionResult<IEnumerable<PermissionAvecStats>>> RechercherPermissions(PermissionRechercheParams parametres)
        {
            var permissions = await _permissionGestionService.RechercherPermissions(parametres);
            return Ok(permissions);
        }

        /// <summary>
        /// Assigne une permission à un rôle
        /// </summary>
        [HttpPost("{id}/assigner-role/{roleId}")]
        public async Task<IActionResult> AssignerPermissionRole(long id, long roleId)
        {
            try
            {
                var success = await _permissionGestionService.AssignerPermissionRole(id, roleId);
                if (!success)
                {
                    return BadRequest("Erreur lors de l'assignation");
                }
                return NoContent();
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Retire une permission d'un rôle
        /// </summary>
        [HttpDelete("{id}/retirer-role/{roleId}")]
        public async Task<IActionResult> RetirerPermissionRole(long id, long roleId)
        {
            try
            {
                var success = await _permissionGestionService.RetirerPermissionRole(id, roleId);
                if (!success)
                {
                    return BadRequest("Erreur lors du retrait");
                }
                return NoContent();
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Copie les permissions d'un rôle vers un autre
        /// </summary>
        [HttpPost("copier-vers-role")]
        public async Task<ActionResult<int>> CopierPermissionsVersRole(CopierPermissionsDto dto)
        {
            try
            {
                var nbCopiees = await _permissionGestionService.CopierPermissionsVersRole(dto);
                return Ok(new { PermissionsCopiees = nbCopiees });
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region Endpoints utilisant les fonctions via service

        /// <summary>
        /// Vérifie si une permission est utilisée
        /// </summary>
        [HttpGet("{id}/est-utilisee")]
        public async Task<ActionResult<bool>> PermissionEstUtilisee(long id)
        {
            return await _permissionGestionService.PermissionEstUtilisee(id);
        }

        /// <summary>
        /// Obtient le nombre de rôles ayant cette permission
        /// </summary>
        [HttpGet("{id}/nb-roles")]
        public async Task<ActionResult<int>> GetNbRoles(long id)
        {
            return await _permissionGestionService.ObtenirNbRoles(id);
        }

        /// <summary>
        /// Obtient le nombre d'utilisateurs ayant cette permission
        /// </summary>
        [HttpGet("{id}/nb-utilisateurs")]
        public async Task<ActionResult<int>> GetNbUtilisateurs(long id)
        {
            return await _permissionGestionService.ObtenirNbUtilisateurs(id);
        }

        /// <summary>
        /// Obtient les permissions d'un rôle (via fonction TVF)
        /// </summary>
        [HttpGet("par-role/{roleId}")]
        public async Task<ActionResult<IEnumerable<PermissionRoleInfo>>> GetPermissionsParRoleTvf(long roleId)
        {
            var permissions = await _permissionGestionService.ObtenirPermissionsRole(roleId);
            return Ok(permissions);
        }

        /// <summary>
        /// Obtient les rôles ayant une permission spécifique
        /// </summary>
        [HttpGet("{id}/roles")]
        public async Task<ActionResult<IEnumerable<RoleAvecPermissionDto>>> GetRolesPermission(long id)
        {
            var roles = await _permissionGestionService.ObtenirRolesPermission(id);
            return Ok(roles);
        }

        /// <summary>
        /// Obtient les utilisateurs ayant une permission spécifique
        /// </summary>
        [HttpGet("{id}/utilisateurs")]
        public async Task<ActionResult<IEnumerable<UtilisateurAvecPermissionDto>>> GetUtilisateursPermission(long id)
        {
            var utilisateurs = await _permissionGestionService.ObtenirUtilisateursPermission(id);
            return Ok(utilisateurs);
        }

        #endregion
    }
}
