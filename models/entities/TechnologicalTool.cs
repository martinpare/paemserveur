using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("technologicalTools")]
    public class TechnologicalTool
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

        [Column("icon")]
        [StringLength(100)]
        public string Icon { get; set; }

        [Required]
        [Column("displayOrder")]
        public int DisplayOrder { get; set; }

        [Required]
        [Column("isActive")]
        public bool IsActive { get; set; } = true;

        [Required]
        [Column("adaptiveMeasure")]
        public bool AdaptiveMeasure { get; set; } = false;

        [Column("defaultData")]
        public string DefaultData { get; set; }

        [Required]
        [Column("interface")]
        public bool Interface { get; set; } = false;

        [Column("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [Column("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
    }
}
