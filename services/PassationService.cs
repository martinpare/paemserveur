using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using serveur.Models;

namespace serveur.Services
{
    public interface IPassationService
    {
        // Opérations principales
        Task<PassationSauvegardeResultat> SauvegarderPassation(PassationSauvegardeDto dto);
        Task<PassationCompleteDto> ObtenirPassation(string passationId);
        Task<PassationCompleteDto> ObtenirPassationEnCours(string etudiantId, string examenId = null);
        Task<IEnumerable<PassationCompleteDto>> RechercherPassations(PassationRechercheParams parametres);

        // Synchronisation
        Task<SynchronisationResultat> SynchroniserOperations(LotOperationsSyncDto lot);
        Task<EtatSynchronisationDto> VerifierEtatSynchronisation(string passationId, int versionClient);

        // Reprise
        Task<PassationRepriseResultat> VerifierReprise(string etudiantId, string examenId);

        // Opérations spécifiques
        Task<bool> EnregistrerReponse(ReponseDto dto);
        Task<bool> ChangerStatut(string passationId, string nouveauStatut, int version);
        Task<int> ObtenirVersionServeur(string passationId);
    }

    public class PassationService : IPassationService
    {
        private readonly string _connectionString;

        public PassationService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        #region Opérations principales

        /// <summary>
        /// Sauvegarde une passation via la procédure stockée SP_SauvegarderPassation
        /// </summary>
        public async Task<PassationSauvegardeResultat> SauvegarderPassation(PassationSauvegardeDto dto)
        {
            using var connection = new SqlConnection(_connectionString);

            var result = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "SP_SauvegarderPassation",
                new
                {
                    Id = dto.Id,
                    Version = dto.Version,
                    ExamenId = dto.ExamenId,
                    EtudiantId = dto.EtudiantId,
                    Statut = dto.Statut,
                    DateDebut = dto.DateDebut,
                    DateFin = dto.DateFin,
                    TempsPauseTotalSec = dto.TempsPauseTotalSec,
                    NombrePauses = dto.NombrePauses,
                    TempsActifSec = dto.TempsActifSec,
                    NombreDeconnexions = dto.NombreDeconnexions,
                    Reponses = dto.Reponses,
                    Configuration = dto.Configuration,
                    HistoriquePauses = dto.HistoriquePauses,
                    HistoriqueEvenements = dto.HistoriqueEvenements,
                    DerniereActivite = dto.DerniereActivite
                },
                commandType: CommandType.StoredProcedure);

            return new PassationSauvegardeResultat
            {
                Succes = result?.Resultat != "CONFLIT_VERSION",
                NouvelleVersion = (int)(result?.NouvelleVersion ?? dto.Version),
                Resultat = result?.Resultat ?? "ERREUR",
                Message = result?.Resultat == "CONFLIT_VERSION"
                    ? "Conflit de version détecté. La version serveur est plus récente."
                    : "Sauvegarde réussie."
            };
        }

        /// <summary>
        /// Obtient une passation par son ID
        /// </summary>
        public async Task<PassationCompleteDto> ObtenirPassation(string passationId)
        {
            using var connection = new SqlConnection(_connectionString);

            return await connection.QueryFirstOrDefaultAsync<PassationCompleteDto>(
                @"SELECT Id, Version, ExamenId, EtudiantId, Statut, DateDebut, DateFin,
                         TempsPauseTotalSec, NombrePauses, TempsActifSec, NombreDeconnexions,
                         Reponses, Configuration, HistoriquePauses, HistoriqueEvenements,
                         DerniereActivite, DerniereSauvegarde, NombreSauvegardes,
                         DateCreation, DateModification
                  FROM Passations
                  WHERE Id = @passationId",
                new { passationId });
        }

        /// <summary>
        /// Obtient la passation en cours pour un étudiant via SP_ObtenirPassationEnCours
        /// </summary>
        public async Task<PassationCompleteDto> ObtenirPassationEnCours(string etudiantId, string examenId = null)
        {
            using var connection = new SqlConnection(_connectionString);

            return await connection.QueryFirstOrDefaultAsync<PassationCompleteDto>(
                "SP_ObtenirPassationEnCours",
                new { EtudiantId = etudiantId, ExamenId = examenId },
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Recherche des passations avec filtres
        /// </summary>
        public async Task<IEnumerable<PassationCompleteDto>> RechercherPassations(PassationRechercheParams parametres)
        {
            using var connection = new SqlConnection(_connectionString);

            var sql = @"SELECT Id, Version, ExamenId, EtudiantId, Statut, DateDebut, DateFin,
                               TempsPauseTotalSec, NombrePauses, TempsActifSec, NombreDeconnexions,
                               Reponses, Configuration, HistoriquePauses, HistoriqueEvenements,
                               DerniereActivite, DerniereSauvegarde, NombreSauvegardes,
                               DateCreation, DateModification
                        FROM Passations
                        WHERE (@ExamenId IS NULL OR ExamenId = @ExamenId)
                          AND (@EtudiantId IS NULL OR EtudiantId = @EtudiantId)
                          AND (@Statut IS NULL OR Statut = @Statut)
                          AND (@DateDebutMin IS NULL OR DateDebut >= @DateDebutMin)
                          AND (@DateDebutMax IS NULL OR DateDebut <= @DateDebutMax)
                        ORDER BY DateCreation DESC";

            return await connection.QueryAsync<PassationCompleteDto>(sql, parametres);
        }

        #endregion

        #region Synchronisation

        /// <summary>
        /// Synchronise un lot d'opérations provenant du client
        /// </summary>
        public async Task<SynchronisationResultat> SynchroniserOperations(LotOperationsSyncDto lot)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            var resultat = new SynchronisationResultat
            {
                OperationsTraitees = 0,
                OperationsIgnorees = 0,
                OperationsEnErreur = new List<string>()
            };

            try
            {
                // Obtenir la version actuelle du serveur
                var versionServeur = await connection.ExecuteScalarAsync<int>(
                    "SELECT ISNULL(Version, 0) FROM Passations WHERE Id = @PassationId",
                    new { lot.PassationId },
                    transaction);

                foreach (var operation in lot.Operations)
                {
                    // Ignorer les opérations déjà traitées (version <= serveur)
                    if (operation.Version <= versionServeur)
                    {
                        resultat.OperationsIgnorees++;
                        continue;
                    }

                    try
                    {
                        // Insérer l'opération dans la file
                        await connection.ExecuteAsync(
                            @"INSERT INTO FileOperationsSynchronisation
                              (Id, PassationId, TypeOperation, VersionSource, Donnees, HorodatageClient, Statut)
                              VALUES (@Id, @PassationId, @TypeOperation, @Version, @Donnees, @HorodatageClient, 'traite')",
                            new
                            {
                                operation.Id,
                                operation.PassationId,
                                operation.TypeOperation,
                                operation.Version,
                                operation.Donnees,
                                operation.HorodatageClient
                            },
                            transaction);

                        resultat.OperationsTraitees++;
                    }
                    catch (Exception ex)
                    {
                        resultat.OperationsEnErreur.Add($"{operation.Id}: {ex.Message}");
                    }
                }

                // Mettre à jour la version de la passation
                if (resultat.OperationsTraitees > 0)
                {
                    var maxVersion = lot.Operations.Count > 0
                        ? lot.Operations[^1].Version
                        : versionServeur;

                    await connection.ExecuteAsync(
                        @"UPDATE Passations
                          SET Version = @Version,
                              DerniereSauvegarde = GETUTCDATE(),
                              NombreSauvegardes = NombreSauvegardes + 1
                          WHERE Id = @PassationId AND Version < @Version",
                        new { lot.PassationId, Version = maxVersion },
                        transaction);

                    resultat.NouvelleVersionServeur = maxVersion;
                }
                else
                {
                    resultat.NouvelleVersionServeur = versionServeur;
                }

                transaction.Commit();
                resultat.Succes = true;
            }
            catch (Exception)
            {
                transaction.Rollback();
                resultat.Succes = false;
                throw;
            }

            return resultat;
        }

        /// <summary>
        /// Vérifie l'état de synchronisation entre le client et le serveur
        /// </summary>
        public async Task<EtatSynchronisationDto> VerifierEtatSynchronisation(string passationId, int versionClient)
        {
            using var connection = new SqlConnection(_connectionString);

            var passation = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "SELECT Version, DerniereSauvegarde FROM Passations WHERE Id = @passationId",
                new { passationId });

            if (passation == null)
            {
                return new EtatSynchronisationDto
                {
                    PassationId = passationId,
                    VersionServeur = 0,
                    DerniereSauvegarde = null,
                    EstSynchronise = false
                };
            }

            return new EtatSynchronisationDto
            {
                PassationId = passationId,
                VersionServeur = passation.Version,
                DerniereSauvegarde = passation.DerniereSauvegarde,
                EstSynchronise = versionClient >= passation.Version
            };
        }

        #endregion

        #region Reprise

        /// <summary>
        /// Vérifie si une passation doit être reprise après une déconnexion
        /// </summary>
        public async Task<PassationRepriseResultat> VerifierReprise(string etudiantId, string examenId)
        {
            var passation = await ObtenirPassationEnCours(etudiantId, examenId);

            if (passation == null)
            {
                return new PassationRepriseResultat
                {
                    PassationTrouvee = false,
                    DoitReprendre = false,
                    Message = "Aucune passation en cours trouvée."
                };
            }

            // Vérifier si la passation est toujours active
            var doitReprendre = passation.Statut == StatutPassation.EnCours ||
                                passation.Statut == StatutPassation.Pause;

            return new PassationRepriseResultat
            {
                PassationTrouvee = true,
                DoitReprendre = doitReprendre,
                Passation = passation,
                Message = doitReprendre
                    ? "Passation en cours détectée. Reprise recommandée."
                    : "Passation terminée ou soumise."
            };
        }

        #endregion

        #region Opérations spécifiques

        /// <summary>
        /// Enregistre une réponse individuelle
        /// </summary>
        public async Task<bool> EnregistrerReponse(ReponseDto dto)
        {
            using var connection = new SqlConnection(_connectionString);

            // Récupérer les réponses actuelles
            var reponsesJson = await connection.ExecuteScalarAsync<string>(
                "SELECT Reponses FROM Passations WHERE Id = @PassationId",
                new { dto.PassationId });

            // Mettre à jour les réponses (logique simplifiée, à adapter selon le format JSON)
            var result = await connection.ExecuteAsync(
                @"UPDATE Passations
                  SET Version = @Version,
                      DerniereActivite = @DateModification,
                      DateModification = GETUTCDATE()
                  WHERE Id = @PassationId AND Version < @Version",
                new { dto.PassationId, dto.Version, dto.DateModification });

            return result > 0;
        }

        /// <summary>
        /// Change le statut d'une passation
        /// </summary>
        public async Task<bool> ChangerStatut(string passationId, string nouveauStatut, int version)
        {
            using var connection = new SqlConnection(_connectionString);

            var result = await connection.ExecuteAsync(
                @"UPDATE Passations
                  SET Statut = @nouveauStatut,
                      Version = @version,
                      DateModification = GETUTCDATE(),
                      DateFin = CASE WHEN @nouveauStatut IN ('termine', 'soumis', 'annule') THEN GETUTCDATE() ELSE DateFin END
                  WHERE Id = @passationId AND Version < @version",
                new { passationId, nouveauStatut, version });

            return result > 0;
        }

        /// <summary>
        /// Obtient la version actuelle du serveur pour une passation
        /// </summary>
        public async Task<int> ObtenirVersionServeur(string passationId)
        {
            using var connection = new SqlConnection(_connectionString);

            return await connection.ExecuteScalarAsync<int>(
                "SELECT ISNULL(Version, 0) FROM Passations WHERE Id = @passationId",
                new { passationId });
        }

        #endregion
    }
}
