using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models
{
    [Table("FileOperationsSynchronisation")]
    public class FileOperationSynchronisation
    {
        [Key]
        [Column("Id")]
        [StringLength(36)]
        public string Id { get; set; }

        [Required]
        [Column("PassationId")]
        [StringLength(36)]
        public string PassationId { get; set; }

        [Required]
        [Column("TypeOperation")]
        [StringLength(50)]
        public string TypeOperation { get; set; }

        [Column("VersionSource")]
        public int VersionSource { get; set; }

        [Required]
        [Column("Donnees")]
        public string Donnees { get; set; }

        [Column("HorodatageClient")]
        public DateTime HorodatageClient { get; set; }

        [Column("HorodatageServeur")]
        public DateTime HorodatageServeur { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("Statut")]
        [StringLength(20)]
        public string Statut { get; set; } = "recu";

        // Navigation property
        [ForeignKey("PassationId")]
        public virtual Passation Passation { get; set; }
    }

    /// <summary>
    /// Types d'opérations de synchronisation
    /// </summary>
    public static class TypeOperationSync
    {
        public const string Reponse = "REPONSE";
        public const string Pause = "PAUSE";
        public const string Reprise = "REPRISE";
        public const string Evenement = "EVENEMENT";
        public const string Soumission = "SOUMISSION";
        public const string Demarrage = "DEMARRAGE";
        public const string Terminaison = "TERMINAISON";
    }

    /// <summary>
    /// Statuts de traitement d'une opération
    /// </summary>
    public static class StatutOperationSync
    {
        public const string Recu = "recu";
        public const string Traite = "traite";
        public const string Ignore = "ignore";
    }
}
