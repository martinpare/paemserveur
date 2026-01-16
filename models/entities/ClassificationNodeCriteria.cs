using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("classificationNodeCriteria")]
    public class ClassificationNodeCriteria
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("classificationNodeId")]
        public int ClassificationNodeId { get; set; }

        [Required]
        [Column("nameFr")]
        [StringLength(500)]
        public string NameFr { get; set; }

        [Column("nameEn")]
        [StringLength(500)]
        public string NameEn { get; set; }

        [Required]
        [Column("isLegalObligation")]
        public bool IsLegalObligation { get; set; }

        [Column("examplesFr")]
        public string ExamplesFr { get; set; }

        [Column("examplesEn")]
        public string ExamplesEn { get; set; }

        [Required]
        [Column("sortOrder")]
        public int SortOrder { get; set; }

        // Navigation properties
        [ForeignKey("ClassificationNodeId")]
        public virtual ClassificationNode ClassificationNode { get; set; }
    }
}
