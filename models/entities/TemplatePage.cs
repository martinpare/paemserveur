using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("templatePages")]
    public class TemplatePage
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("templateDocumentId")]
        public int TemplateDocumentId { get; set; }

        [Required]
        [Column("sortOrder")]
        public int SortOrder { get; set; }

        [Column("pageNameFr")]
        [StringLength(100)]
        public string PageNameFr { get; set; }

        [Column("pageNameEn")]
        [StringLength(100)]
        public string PageNameEn { get; set; }

        // Navigation properties
        [ForeignKey("TemplateDocumentId")]
        public virtual Document TemplateDocument { get; set; }
    }
}
