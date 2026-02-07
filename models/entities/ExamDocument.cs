using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("examDocuments")]
    public class ExamDocument
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("examId")]
        public int ExamId { get; set; }

        [Required]
        [Column("documentId")]
        public int DocumentId { get; set; }

        [Column("displayOrder")]
        public int DisplayOrder { get; set; }

        [Column("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [Column("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("ExamId")]
        public virtual Exam Exam { get; set; }

        [ForeignKey("DocumentId")]
        public virtual Document Document { get; set; }
    }
}
