using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using serveur.Models;

namespace serveur.Services
{
    public interface IRessourceService
    {
        // Procédures stockées
        Task<long> CreerRessource(RessourceCreationDto dto);
        Task<bool> ModifierRessource(long id, RessourceCreationDto dto);
        Task<bool> SupprimerRessource(long ressourceId, bool force = false);
        Task<long> DupliquerRessource(long ressourceId, RessourceDuplicationDto dto);
        Task<RessourceAvecStats> ObtenirRessourceComplete(long ressourceId);
        Task<IEnumerable<RessourceAvecStats>> RechercherRessources(RessourceRechercheParams parametres);

        // Fonctions
        Task<bool> RessourceEstUtilisee(long ressourceId);
        Task<int> ObtenirNbPermissions(long ressourceId);
        Task<IEnumerable<RessourceUtilisateurDto>> ObtenirRessourcesUtilisateur(long utilisateurId);
        Task<IEnumerable<RessourceAvecStats>> ObtenirRessourcesParType(string type);
    }

    public class RessourceService : IRessourceService
    {
        private readonly string _connectionString;

        public RessourceService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        #region Procédures stockées

        /// <summary>
        /// Crée une nouvelle ressource via sp_creer_ressource
        /// </summary>
        public async Task<long> CreerRessource(RessourceCreationDto dto)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@type", dto.Type);
            parameters.Add("@code", dto.Code);
            parameters.Add("@nom", dto.Nom);
            parameters.Add("@description", dto.Description);
            parameters.Add("@id", dbType: DbType.Int64, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "sp_creer_ressource",
                parameters,
                commandType: CommandType.StoredProcedure);

            return parameters.Get<long>("@id");
        }

        /// <summary>
        /// Modifie une ressource existante via sp_modifier_ressource
        /// </summary>
        public async Task<bool> ModifierRessource(long id, RessourceCreationDto dto)
        {
            using var connection = new SqlConnection(_connectionString);
            var result = await connection.ExecuteAsync(
                "sp_modifier_ressource",
                new
                {
                    id,
                    type = dto.Type,
                    code = dto.Code,
                    nom = dto.Nom,
                    description = dto.Description
                },
                commandType: CommandType.StoredProcedure);

            return result >= 0;
        }

        /// <summary>
        /// Supprime une ressource via sp_supprimer_ressource
        /// </summary>
        public async Task<bool> SupprimerRessource(long ressourceId, bool force = false)
        {
            using var connection = new SqlConnection(_connectionString);
            var result = await connection.ExecuteAsync(
                "sp_supprimer_ressource",
                new { ressource_id = ressourceId, force },
                commandType: CommandType.StoredProcedure);

            return result >= 0;
        }

        /// <summary>
        /// Duplique une ressource avec ses permissions via sp_dupliquer_ressource
        /// </summary>
        public async Task<long> DupliquerRessource(long ressourceId, RessourceDuplicationDto dto)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@ressource_id", ressourceId);
            parameters.Add("@nouveau_code", dto.NouveauCode);
            parameters.Add("@nouveau_nom", dto.NouveauNom);
            parameters.Add("@nouvelle_ressource_id", dbType: DbType.Int64, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "sp_dupliquer_ressource",
                parameters,
                commandType: CommandType.StoredProcedure);

            return parameters.Get<long>("@nouvelle_ressource_id");
        }

        /// <summary>
        /// Obtient une ressource avec toutes ses statistiques via sp_obtenir_ressource_complete
        /// </summary>
        public async Task<RessourceAvecStats> ObtenirRessourceComplete(long ressourceId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<RessourceAvecStats>(
                "sp_obtenir_ressource_complete",
                new { ressource_id = ressourceId },
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Recherche des ressources avec filtres via sp_rechercher_ressources
        /// </summary>
        public async Task<IEnumerable<RessourceAvecStats>> RechercherRessources(RessourceRechercheParams parametres)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<RessourceAvecStats>(
                "sp_rechercher_ressources",
                new
                {
                    type = parametres.Type,
                    code_contient = parametres.CodeContient,
                    nom_contient = parametres.NomContient,
                    uniquement_utilisees = parametres.UniquementUtilisees
                },
                commandType: CommandType.StoredProcedure);
        }

        #endregion

        #region Fonctions

        /// <summary>
        /// Vérifie si une ressource est utilisée via fn_ressource_est_utilisee
        /// </summary>
        public async Task<bool> RessourceEstUtilisee(long ressourceId)
        {
            using var connection = new SqlConnection(_connectionString);
            var result = await connection.ExecuteScalarAsync<bool>(
                "SELECT dbo.fn_ressource_est_utilisee(@ressourceId)",
                new { ressourceId });
            return result;
        }

        /// <summary>
        /// Obtient le nombre de permissions liées à une ressource via fn_ressource_nb_permissions
        /// </summary>
        public async Task<int> ObtenirNbPermissions(long ressourceId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.ExecuteScalarAsync<int>(
                "SELECT dbo.fn_ressource_nb_permissions(@ressourceId)",
                new { ressourceId });
        }

        /// <summary>
        /// Obtient les ressources accessibles par un utilisateur via fn_obtenir_ressources_utilisateur
        /// </summary>
        public async Task<IEnumerable<RessourceUtilisateurDto>> ObtenirRessourcesUtilisateur(long utilisateurId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<RessourceUtilisateurDto>(
                "SELECT * FROM dbo.fn_obtenir_ressources_utilisateur(@utilisateurId)",
                new { utilisateurId });
        }

        /// <summary>
        /// Obtient les ressources filtrées par type via fn_obtenir_ressources_par_type
        /// </summary>
        public async Task<IEnumerable<RessourceAvecStats>> ObtenirRessourcesParType(string type)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<RessourceAvecStats>(
                "SELECT * FROM dbo.fn_obtenir_ressources_par_type(@type)",
                new { type });
        }

        #endregion
    }
}
