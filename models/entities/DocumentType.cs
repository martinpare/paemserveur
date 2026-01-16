using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("documentTypes")]
    public class DocumentType
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("pedagogicalStrudtureId")]
        public int? PedagogicalStructureId { get; set; }

        [Required]
        [Column("code")]
        [StringLength(50)]
        public string Code { get; set; }

        [Required]
        [Column("titleFr")]
        [StringLength(255)]
        public string TitleFr { get; set; }

        [Required]
        [Column("titleEn")]
        [StringLength(255)]
        public string TitleEn { get; set; }

        [Column("descriptionFr")]
        public string DescriptionFr { get; set; }

        [Column("descriptionEn")]
        public string DescriptionEn { get; set; }

        // Navigation properties
        [ForeignKey("PedagogicalStructureId")]
        public virtual PedagogicalStructure PedagogicalStructure { get; set; }
    }
}
