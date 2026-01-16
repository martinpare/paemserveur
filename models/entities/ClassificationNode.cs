using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("classificationNodes")]
    public class ClassificationNode
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("classificationId")]
        public int ClassificationId { get; set; }

        [Column("parentId")]
        public int? ParentId { get; set; }

        [Required]
        [Column("label")]
        [StringLength(100)]
        public string Label { get; set; }

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

        [Column("weight")]
        [StringLength(50)]
        public string Weight { get; set; }

        [Column("referencesJuridiques")]
        [StringLength(2000)]
        public string ReferencesJuridiques { get; set; }

        // Navigation properties
        [ForeignKey("ClassificationId")]
        public virtual Classification Classification { get; set; }

        [ForeignKey("ParentId")]
        public virtual ClassificationNode Parent { get; set; }
    }
}
