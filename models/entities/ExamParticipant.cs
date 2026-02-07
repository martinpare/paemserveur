using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("examParticipants")]
    public class ExamParticipant
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("examSessionId")]
        public int ExamSessionId { get; set; }

        [Required]
        [Column("learnerId")]
        public int LearnerId { get; set; }

        [Column("convocationId")]
        public int? ConvocationId { get; set; }

        [Required]
        [Column("status")]
        [StringLength(50)]
        public string Status { get; set; }

        [Column("connectionId")]
        [StringLength(100)]
        public string ConnectionId { get; set; }

        [Column("remainingTimeSeconds")]
        public int? RemainingTimeSeconds { get; set; }

        [Column("additionalTimeMinutes")]
        public int AdditionalTimeMinutes { get; set; }

        [Column("startedAt")]
        public DateTime? StartedAt { get; set; }

        [Column("submittedAt")]
        public DateTime? SubmittedAt { get; set; }

        [Required]
        [Column("createdAt")]
        public DateTime CreatedAt { get; set; }

        [Required]
        [Column("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("ExamSessionId")]
        public virtual ExamSession ExamSession { get; set; }

        [ForeignKey("LearnerId")]
        public virtual Learner Learner { get; set; }

        [ForeignKey("ConvocationId")]
        public virtual Convocation Convocation { get; set; }

        public virtual ICollection<ExamIncident> Incidents { get; set; }
        public virtual ICollection<ExamLog> Logs { get; set; }
    }
}
