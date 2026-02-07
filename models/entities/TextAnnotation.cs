using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("textAnnotations")]
    public class TextAnnotation
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("learnerId")]
        public int LearnerId { get; set; }

        [Required]
        [Column("contextId")]
        [StringLength(255)]
        public string ContextId { get; set; }

        [Required]
        [Column("annotationType")]
        [StringLength(50)]
        public string AnnotationType { get; set; }

        [Required]
        [Column("color")]
        [StringLength(20)]
        public string Color { get; set; }

        [Required]
        [Column("textContent")]
        public string TextContent { get; set; }

        [Required]
        [Column("xPath")]
        [StringLength(1000)]
        public string XPath { get; set; }

        [Required]
        [Column("startOffset")]
        public int StartOffset { get; set; }

        [Required]
        [Column("endOffset")]
        public int EndOffset { get; set; }

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("LearnerId")]
        public virtual Learner Learner { get; set; }
    }
}
