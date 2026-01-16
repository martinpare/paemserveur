using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("classifications")]
    public class Classification
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("pedagogicalStrudtureId")]
        public int? PedagogicalStructureId { get; set; }

        [Column("tag")]
        [StringLength(100)]
        public string Tag { get; set; }

        [Required]
        [Column("nameFr")]
        [StringLength(255)]
        public string NameFr { get; set; }

        [Required]
        [Column("nameEn")]
        [StringLength(255)]
        public string NameEn { get; set; }

        [Required]
        [Column("descriptionFr")]
        [StringLength(500)]
        public string DescriptionFr { get; set; }

        [Required]
        [Column("descriptionEn")]
        [StringLength(500)]
        public string DescriptionEn { get; set; }

        [Required]
        [Column("isRequired")]
        public bool IsRequired { get; set; }

        [Required]
        [Column("allowMultiple")]
        public bool AllowMultiple { get; set; }

        [Required]
        [Column("hasDescription")]
        public bool HasDescription { get; set; }

        [Required]
        [Column("isActive")]
        public bool IsActive { get; set; }

        // Navigation properties
        [ForeignKey("PedagogicalStructureId")]
        public virtual PedagogicalStructure PedagogicalStructure { get; set; }
    }
}
