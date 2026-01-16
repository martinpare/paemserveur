using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("documentElements")]
    public class DocumentElement
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("documentId")]
        public int DocumentId { get; set; }

        [Column("sortOrder")]
        public int? SortOrder { get; set; }

        [Required]
        [Column("elementType")]
        [StringLength(50)]
        public string ElementType { get; set; }

        [Column("content")]
        public string Content { get; set; }

        [Column("templatePageId")]
        public int? TemplatePageId { get; set; }

        [Column("styleProps")]
        public string StyleProps { get; set; }

        // Navigation properties
        [ForeignKey("DocumentId")]
        public virtual Document Document { get; set; }

        [ForeignKey("TemplatePageId")]
        public virtual TemplatePage TemplatePage { get; set; }
    }
}
