using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using serveur.Models;

namespace serveur.Services
{
    public interface IPermissionGestionService
    {
        // Procédures stockées
        Task<CreerPermissionResult> CreerPermission(CreerPermissionDto dto);
        Task<bool> ModifierPermission(long id, ModifierPermissionDto dto);
        Task<bool> SupprimerPermission(long permissionId, bool force = false);
        Task<IEnumerable<CreerPermissionsCrudResult>> CreerPermissionsCrud(long ressourceId, string prefixeCode = null);
        Task<long> DupliquerPermission(long permissionId, PermissionDuplicationDto dto);
        Task<PermissionAvecStats> ObtenirPermissionComplete(long permissionId);
        Task<IEnumerable<PermissionAvecStats>> RechercherPermissions(PermissionRechercheParams parametres);
        Task<bool> AssignerPermissionRole(long permissionId, long roleId);
        Task<bool> RetirerPermissionRole(long permissionId, long roleId);
        Task<int> CopierPermissionsVersRole(CopierPermissionsDto dto);

        // Fonctions
        Task<bool> PermissionEstUtilisee(long permissionId);
        Task<int> ObtenirNbRoles(long permissionId);
        Task<int> ObtenirNbUtilisateurs(long permissionId);
        Task<IEnumerable<PermissionRoleInfo>> ObtenirPermissionsRole(long roleId);
        Task<IEnumerable<PermissionAvecStats>> ObtenirPermissionsParAction(string action);
        Task<IEnumerable<RoleAvecPermissionDto>> ObtenirRolesPermission(long permissionId);
        Task<IEnumerable<UtilisateurAvecPermissionDto>> ObtenirUtilisateursPermission(long permissionId);
    }

    public class PermissionGestionService : IPermissionGestionService
    {
        private readonly string _connectionString;

        public PermissionGestionService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        #region Procédures stockées

        /// <summary>
        /// Crée une nouvelle permission via sp_creer_permission
        /// </summary>
        public async Task<CreerPermissionResult> CreerPermission(CreerPermissionDto dto)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<CreerPermissionResult>(
                "sp_creer_permission",
                new
                {
                    code = dto.Code,
                    action = dto.Action,
                    ressource_id = dto.RessourceId,
                    description = dto.Description
                },
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Modifie une permission existante via sp_modifier_permission
        /// </summary>
        public async Task<bool> ModifierPermission(long id, ModifierPermissionDto dto)
        {
            using var connection = new SqlConnection(_connectionString);
            var result = await connection.ExecuteAsync(
                "sp_modifier_permission",
                new
                {
                    id,
                    code = dto.Code,
                    action = dto.Action,
                    ressource_id = dto.RessourceId,
                    description = dto.Description
                },
                commandType: CommandType.StoredProcedure);

            return result >= 0;
        }

        /// <summary>
        /// Supprime une permission via sp_supprimer_permission
        /// </summary>
        public async Task<bool> SupprimerPermission(long permissionId, bool force = false)
        {
            using var connection = new SqlConnection(_connectionString);
            var result = await connection.ExecuteAsync(
                "sp_supprimer_permission",
                new { permission_id = permissionId, force },
                commandType: CommandType.StoredProcedure);

            return result >= 0;
        }

        /// <summary>
        /// Crée les 4 permissions CRUD pour une ressource via sp_creer_permissions_crud
        /// </summary>
        public async Task<IEnumerable<CreerPermissionsCrudResult>> CreerPermissionsCrud(long ressourceId, string prefixeCode = null)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<CreerPermissionsCrudResult>(
                "sp_creer_permissions_crud",
                new { ressource_id = ressourceId, prefixe_code = prefixeCode },
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Duplique une permission via sp_dupliquer_permission
        /// </summary>
        public async Task<long> DupliquerPermission(long permissionId, PermissionDuplicationDto dto)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@permission_id", permissionId);
            parameters.Add("@nouveau_code", dto.NouveauCode);
            parameters.Add("@nouvelle_ressource_id", dto.NouvelleRessourceId);
            parameters.Add("@nouvelle_permission_id", dbType: DbType.Int64, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "sp_dupliquer_permission",
                parameters,
                commandType: CommandType.StoredProcedure);

            return parameters.Get<long>("@nouvelle_permission_id");
        }

        /// <summary>
        /// Obtient une permission avec toutes ses statistiques via sp_obtenir_permission_complete
        /// </summary>
        public async Task<PermissionAvecStats> ObtenirPermissionComplete(long permissionId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<PermissionAvecStats>(
                "sp_obtenir_permission_complete",
                new { permission_id = permissionId },
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Recherche des permissions avec filtres via sp_rechercher_permissions
        /// </summary>
        public async Task<IEnumerable<PermissionAvecStats>> RechercherPermissions(PermissionRechercheParams parametres)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<PermissionAvecStats>(
                "sp_rechercher_permissions",
                new
                {
                    action = parametres.Action,
                    ressource_type = parametres.RessourceType,
                    code_contient = parametres.CodeContient,
                    uniquement_utilisees = parametres.UniquementUtilisees,
                    uniquement_orphelines = parametres.UniquementOrphelines
                },
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Assigne une permission à un rôle via sp_assigner_permission_role
        /// </summary>
        public async Task<bool> AssignerPermissionRole(long permissionId, long roleId)
        {
            using var connection = new SqlConnection(_connectionString);
            var result = await connection.ExecuteAsync(
                "sp_assigner_permission_role",
                new { permission_id = permissionId, role_id = roleId },
                commandType: CommandType.StoredProcedure);

            return result >= 0;
        }

        /// <summary>
        /// Retire une permission d'un rôle via sp_retirer_permission_role
        /// </summary>
        public async Task<bool> RetirerPermissionRole(long permissionId, long roleId)
        {
            using var connection = new SqlConnection(_connectionString);
            var result = await connection.ExecuteAsync(
                "sp_retirer_permission_role",
                new { permission_id = permissionId, role_id = roleId },
                commandType: CommandType.StoredProcedure);

            return result >= 0;
        }

        /// <summary>
        /// Copie toutes les permissions d'un rôle vers un autre via sp_copier_permissions_vers_role
        /// </summary>
        public async Task<int> CopierPermissionsVersRole(CopierPermissionsDto dto)
        {
            using var connection = new SqlConnection(_connectionString);
            var result = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "sp_copier_permissions_vers_role",
                new
                {
                    role_source_id = dto.RoleSourceId,
                    role_cible_id = dto.RoleCibleId,
                    remplacer = dto.Remplacer
                },
                commandType: CommandType.StoredProcedure);

            return result?.PermissionsCopiees ?? 0;
        }

        #endregion

        #region Fonctions

        /// <summary>
        /// Vérifie si une permission est utilisée via fn_permission_est_utilisee
        /// </summary>
        public async Task<bool> PermissionEstUtilisee(long permissionId)
        {
            using var connection = new SqlConnection(_connectionString);
            var result = await connection.ExecuteScalarAsync<bool>(
                "SELECT dbo.fn_permission_est_utilisee(@permissionId)",
                new { permissionId });
            return result;
        }

        /// <summary>
        /// Obtient le nombre de rôles ayant cette permission via fn_permission_nb_roles
        /// </summary>
        public async Task<int> ObtenirNbRoles(long permissionId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.ExecuteScalarAsync<int>(
                "SELECT dbo.fn_permission_nb_roles(@permissionId)",
                new { permissionId });
        }

        /// <summary>
        /// Obtient le nombre d'utilisateurs ayant cette permission via fn_permission_nb_utilisateurs
        /// </summary>
        public async Task<int> ObtenirNbUtilisateurs(long permissionId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.ExecuteScalarAsync<int>(
                "SELECT dbo.fn_permission_nb_utilisateurs(@permissionId)",
                new { permissionId });
        }

        /// <summary>
        /// Obtient les permissions d'un rôle via fn_obtenir_permissions_role
        /// </summary>
        public async Task<IEnumerable<PermissionRoleInfo>> ObtenirPermissionsRole(long roleId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<PermissionRoleInfo>(
                "SELECT * FROM dbo.fn_obtenir_permissions_role(@roleId)",
                new { roleId });
        }

        /// <summary>
        /// Obtient les permissions filtrées par action via fn_obtenir_permissions_par_action
        /// </summary>
        public async Task<IEnumerable<PermissionAvecStats>> ObtenirPermissionsParAction(string action)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<PermissionAvecStats>(
                "SELECT * FROM dbo.fn_obtenir_permissions_par_action(@action)",
                new { action });
        }

        /// <summary>
        /// Obtient les rôles ayant une permission via fn_obtenir_roles_permission
        /// </summary>
        public async Task<IEnumerable<RoleAvecPermissionDto>> ObtenirRolesPermission(long permissionId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<RoleAvecPermissionDto>(
                "SELECT * FROM dbo.fn_obtenir_roles_permission(@permissionId)",
                new { permissionId });
        }

        /// <summary>
        /// Obtient les utilisateurs ayant une permission via fn_obtenir_utilisateurs_permission
        /// </summary>
        public async Task<IEnumerable<UtilisateurAvecPermissionDto>> ObtenirUtilisateursPermission(long permissionId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<UtilisateurAvecPermissionDto>(
                "SELECT * FROM dbo.fn_obtenir_utilisateurs_permission(@permissionId)",
                new { permissionId });
        }

        #endregion
    }
}
