using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("examLogs")]
    public class ExamLog
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("examSessionId")]
        public int ExamSessionId { get; set; }

        [Column("participantId")]
        public int? ParticipantId { get; set; }

        [Required]
        [Column("actorType")]
        [StringLength(20)]
        public string ActorType { get; set; }

        [Column("actorId")]
        public int? ActorId { get; set; }

        [Required]
        [Column("action")]
        [StringLength(100)]
        public string Action { get; set; }

        [Column("details")]
        public string Details { get; set; }

        [Required]
        [Column("createdAt")]
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        [ForeignKey("ExamSessionId")]
        public virtual ExamSession ExamSession { get; set; }

        [ForeignKey("ParticipantId")]
        public virtual ExamParticipant Participant { get; set; }
    }
}
