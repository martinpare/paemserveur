using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("convocations")]
    public class Convocation
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("examId")]
        public int ExamId { get; set; }

        [Required]
        [Column("learnerId")]
        public int LearnerId { get; set; }

        [Required]
        [Column("sessionId")]
        public int SessionId { get; set; }

        [Column("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [Column("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [Column("statusId")]
        public int? StatusId { get; set; }

        [Column("additionalTimeMinutes")]
        public int AdditionalTimeMinutes { get; set; } = 0;

        [StringLength(500)]
        [Column("notes")]
        public string Notes { get; set; }

        // Navigation properties
        [ForeignKey("ExamId")]
        public virtual Exam Exam { get; set; }

        [ForeignKey("LearnerId")]
        public virtual Learner Learner { get; set; }

        [ForeignKey("SessionId")]
        public virtual Session Session { get; set; }
    }
}
