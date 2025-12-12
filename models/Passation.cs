using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models
{
    [Table("Passations")]
    public class Passation
    {
        [Key]
        [Column("Id")]
        [StringLength(36)]
        public string Id { get; set; }

        [Column("Version")]
        public int Version { get; set; } = 0;

        [Required]
        [Column("ExamenId")]
        [StringLength(36)]
        public string ExamenId { get; set; }

        [Required]
        [Column("EtudiantId")]
        [StringLength(36)]
        public string EtudiantId { get; set; }

        [Required]
        [Column("Statut")]
        [StringLength(20)]
        public string Statut { get; set; } = "non_demarre";

        [Column("DateDebut")]
        public DateTime? DateDebut { get; set; }

        [Column("DateFin")]
        public DateTime? DateFin { get; set; }

        [Column("TempsPauseTotalSec")]
        public int TempsPauseTotalSec { get; set; } = 0;

        [Column("NombrePauses")]
        public int NombrePauses { get; set; } = 0;

        [Column("TempsActifSec")]
        public int TempsActifSec { get; set; } = 0;

        [Column("NombreDeconnexions")]
        public int NombreDeconnexions { get; set; } = 0;

        [Column("Reponses")]
        public string Reponses { get; set; }

        [Column("Configuration")]
        public string Configuration { get; set; }

        [Column("HistoriquePauses")]
        public string HistoriquePauses { get; set; }

        [Column("HistoriqueEvenements")]
        public string HistoriqueEvenements { get; set; }

        [Column("DerniereActivite")]
        public DateTime? DerniereActivite { get; set; }

        [Column("DerniereSauvegarde")]
        public DateTime? DerniereSauvegarde { get; set; }

        [Column("NombreSauvegardes")]
        public int NombreSauvegardes { get; set; } = 0;

        [Column("DateCreation")]
        public DateTime DateCreation { get; set; } = DateTime.UtcNow;

        [Column("DateModification")]
        public DateTime DateModification { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Statuts possibles d'une passation
    /// </summary>
    public static class StatutPassation
    {
        public const string NonDemarre = "non_demarre";
        public const string EnCours = "en_cours";
        public const string Pause = "pause";
        public const string Termine = "termine";
        public const string Soumis = "soumis";
        public const string Annule = "annule";
    }
}
