using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("reports")]
    public class Report
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("code")]
        [StringLength(100)]
        public string Code { get; set; }

        [Required]
        [Column("nameFr")]
        [StringLength(255)]
        public string NameFr { get; set; }

        [Required]
        [Column("nameEn")]
        [StringLength(255)]
        public string NameEn { get; set; }

        [Column("descriptionFr")]
        [StringLength(1000)]
        public string DescriptionFr { get; set; }

        [Column("descriptionEn")]
        [StringLength(1000)]
        public string DescriptionEn { get; set; }

        [Required]
        [Column("sortOrder")]
        public int SortOrder { get; set; }

        [Required]
        [Column("isActive")]
        public bool IsActive { get; set; }

        [Required]
        [Column("organisationId")]
        public int OrganisationId { get; set; }

        [Column("pedagogicalStrudturId")]
        public int? PedagogicalStructureId { get; set; }

        [Required]
        [Column("useCompression")]
        public bool UseCompression { get; set; }

        [Column("compressionMethod")]
        [StringLength(100)]
        public string CompressionMethod { get; set; }

        [Column("compressionPage")]
        [StringLength(100)]
        public string CompressionPage { get; set; }

        [Required]
        [Column("widgetName")]
        [StringLength(100)]
        public string WidgetName { get; set; }

        [Column("controllerMethod")]
        [StringLength(100)]
        public string ControllerMethod { get; set; }

        // Navigation properties
        [ForeignKey("OrganisationId")]
        public virtual Organisation Organisation { get; set; }

        [ForeignKey("PedagogicalStructureId")]
        public virtual PedagogicalStructure PedagogicalStructure { get; set; }
    }
}
