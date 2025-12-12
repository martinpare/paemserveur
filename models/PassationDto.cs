using System;
using System.Collections.Generic;

namespace serveur.Models
{
    /// <summary>
    /// DTO pour la sauvegarde d'une passation (envoyé par le client)
    /// </summary>
    public class PassationSauvegardeDto
    {
        public string Id { get; set; }
        public int Version { get; set; }
        public string ExamenId { get; set; }
        public string EtudiantId { get; set; }
        public string Statut { get; set; }
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public int TempsPauseTotalSec { get; set; }
        public int NombrePauses { get; set; }
        public int TempsActifSec { get; set; }
        public int NombreDeconnexions { get; set; }
        public string Reponses { get; set; }
        public string Configuration { get; set; }
        public string HistoriquePauses { get; set; }
        public string HistoriqueEvenements { get; set; }
        public DateTime? DerniereActivite { get; set; }
    }

    /// <summary>
    /// DTO pour la création d'une nouvelle passation
    /// </summary>
    public class PassationCreationDto
    {
        public string ExamenId { get; set; }
        public string EtudiantId { get; set; }
        public string Configuration { get; set; }
    }

    /// <summary>
    /// DTO pour l'enregistrement d'une réponse
    /// </summary>
    public class ReponseDto
    {
        public string PassationId { get; set; }
        public int Version { get; set; }
        public string ItemId { get; set; }
        public object Valeur { get; set; }
        public DateTime DateModification { get; set; }
    }

    /// <summary>
    /// DTO pour une opération de synchronisation
    /// </summary>
    public class OperationSyncDto
    {
        public string Id { get; set; }
        public string PassationId { get; set; }
        public string TypeOperation { get; set; }
        public int Version { get; set; }
        public string Donnees { get; set; }
        public DateTime HorodatageClient { get; set; }
    }

    /// <summary>
    /// DTO pour un lot d'opérations à synchroniser
    /// </summary>
    public class LotOperationsSyncDto
    {
        public string PassationId { get; set; }
        public List<OperationSyncDto> Operations { get; set; }
    }

    /// <summary>
    /// Résultat de la sauvegarde d'une passation
    /// </summary>
    public class PassationSauvegardeResultat
    {
        public bool Succes { get; set; }
        public int NouvelleVersion { get; set; }
        public string Resultat { get; set; }  // "INSERE", "MIS_A_JOUR", "CONFLIT_VERSION"
        public string Message { get; set; }
    }

    /// <summary>
    /// Résultat de la synchronisation d'un lot d'opérations
    /// </summary>
    public class SynchronisationResultat
    {
        public bool Succes { get; set; }
        public int OperationsTraitees { get; set; }
        public int OperationsIgnorees { get; set; }
        public int NouvelleVersionServeur { get; set; }
        public List<string> OperationsEnErreur { get; set; }
    }

    /// <summary>
    /// DTO pour la recherche de passations
    /// </summary>
    public class PassationRechercheParams
    {
        public string ExamenId { get; set; }
        public string EtudiantId { get; set; }
        public string Statut { get; set; }
        public DateTime? DateDebutMin { get; set; }
        public DateTime? DateDebutMax { get; set; }
    }

    /// <summary>
    /// DTO complet pour retourner une passation au client
    /// </summary>
    public class PassationCompleteDto
    {
        public string Id { get; set; }
        public int Version { get; set; }
        public string ExamenId { get; set; }
        public string EtudiantId { get; set; }
        public string Statut { get; set; }
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public int TempsPauseTotalSec { get; set; }
        public int NombrePauses { get; set; }
        public int TempsActifSec { get; set; }
        public int NombreDeconnexions { get; set; }
        public string Reponses { get; set; }
        public string Configuration { get; set; }
        public string HistoriquePauses { get; set; }
        public string HistoriqueEvenements { get; set; }
        public DateTime? DerniereActivite { get; set; }
        public DateTime? DerniereSauvegarde { get; set; }
        public int NombreSauvegardes { get; set; }
        public DateTime DateCreation { get; set; }
        public DateTime DateModification { get; set; }
    }

    /// <summary>
    /// DTO pour vérifier l'état de synchronisation
    /// </summary>
    public class EtatSynchronisationDto
    {
        public string PassationId { get; set; }
        public int VersionServeur { get; set; }
        public DateTime? DerniereSauvegarde { get; set; }
        public bool EstSynchronise { get; set; }
    }

    /// <summary>
    /// DTO pour la reprise d'une passation
    /// </summary>
    public class PassationRepriseDto
    {
        public string PassationId { get; set; }
        public int VersionClient { get; set; }
    }

    /// <summary>
    /// Résultat de la vérification de reprise
    /// </summary>
    public class PassationRepriseResultat
    {
        public bool PassationTrouvee { get; set; }
        public bool DoitReprendre { get; set; }
        public PassationCompleteDto Passation { get; set; }
        public string Message { get; set; }
    }
}
