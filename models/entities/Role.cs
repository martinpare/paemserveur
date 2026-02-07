using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("roles")]
    public class Role
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("code")]
        [StringLength(50)]
        public string Code { get; set; }

        [Required]
        [Column("nameFr")]
        [StringLength(100)]
        public string NameFr { get; set; }

        [Required]
        [Column("nameEn")]
        [StringLength(100)]
        public string NameEn { get; set; }

        [Column("descriptionFr")]
        [StringLength(500)]
        public string DescriptionFr { get; set; }

        [Column("descriptionEn")]
        [StringLength(500)]
        public string DescriptionEn { get; set; }

        [Column("organisationId")]
        public int? OrganisationId { get; set; }

        [Column("parentId")]
        public int? ParentId { get; set; }

        [Required]
        [Column("level")]
        public int Level { get; set; }

        [Required]
        [Column("isSystem")]
        public bool IsSystem { get; set; }

        [Required]
        [Column("isActive")]
        public bool IsActive { get; set; }

        [Required]
        [Column("hasAllPermissions")]
        public bool HasAllPermissions { get; set; }

        [Column("showPedagogicalStructureInHeader")]
        public bool ShowPedagogicalStructureInHeader { get; set; } = false;

        [Column("showLearningCenterInHeader")]
        public bool ShowLearningCenterInHeader { get; set; } = false;

        // Navigation properties
        [ForeignKey("OrganisationId")]
        public virtual Organisation Organisation { get; set; }

        [ForeignKey("ParentId")]
        public virtual Role Parent { get; set; }
    }
}
