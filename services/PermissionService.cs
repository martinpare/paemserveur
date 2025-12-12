using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using serveur.Models;

namespace serveur.Services
{
    public interface IPermissionService
    {
        Task<IEnumerable<PermissionUtilisateur>> ObtenirPermissionsUtilisateur(long utilisateurId);
        Task<bool> UtilisateurAPermission(long utilisateurId, string codePermission);
        Task<bool> UtilisateurAPermissionRessource(long utilisateurId, string action, string ressourceCode);
        Task<IEnumerable<PermissionUtilisateur>> ObtenirPermissionsParFonction(long utilisateurId);
    }

    public class PermissionService : IPermissionService
    {
        private readonly string _connectionString;

        public PermissionService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Appelle la procédure stockée sp_obtenir_utilisateur_permissions
        /// La procédure retourne 4 résultats, on lit le 3ème (permissions)
        /// </summary>
        public async Task<IEnumerable<PermissionUtilisateur>> ObtenirPermissionsUtilisateur(long utilisateurId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var multi = await connection.QueryMultipleAsync(
                "sp_obtenir_utilisateur_permissions",
                new { utilisateur_id = utilisateurId },
                commandType: CommandType.StoredProcedure);

            // Résultat 1 : Infos utilisateur (on ignore)
            await multi.ReadAsync();

            // Résultat 2 : Rôles (on ignore)
            await multi.ReadAsync();

            // Résultat 3 : Permissions (ce qu'on veut)
            var permissions = await multi.ReadAsync<PermissionUtilisateur>();

            return permissions;
        }

        /// <summary>
        /// Appelle la fonction fn_utilisateur_a_permission
        /// </summary>
        public async Task<bool> UtilisateurAPermission(long utilisateurId, string codePermission)
        {
            using var connection = new SqlConnection(_connectionString);
            var result = await connection.ExecuteScalarAsync<int>(
                "SELECT dbo.fn_utilisateur_a_permission(@utilisateurId, @codePermission)",
                new { utilisateurId, codePermission });
            return result == 1;
        }

        /// <summary>
        /// Appelle la fonction fn_utilisateur_a_permission_ressource
        /// </summary>
        public async Task<bool> UtilisateurAPermissionRessource(long utilisateurId, string action, string ressourceCode)
        {
            using var connection = new SqlConnection(_connectionString);
            var result = await connection.ExecuteScalarAsync<int>(
                "SELECT dbo.fn_utilisateur_a_permission_ressource(@utilisateurId, @action, @ressourceCode)",
                new { utilisateurId, action, ressourceCode });
            return result == 1;
        }

        /// <summary>
        /// Appelle la fonction table fn_obtenir_permissions_utilisateur
        /// </summary>
        public async Task<IEnumerable<PermissionUtilisateur>> ObtenirPermissionsParFonction(long utilisateurId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<PermissionUtilisateur>(
                "SELECT * FROM dbo.fn_obtenir_permissions_utilisateur(@utilisateurId)",
                new { utilisateurId });
        }
    }
}
