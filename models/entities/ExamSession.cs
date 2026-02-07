using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("examSessions")]
    public class ExamSession
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("sessionId")]
        public int SessionId { get; set; }

        [Required]
        [Column("groupCode")]
        [StringLength(20)]
        public string GroupCode { get; set; }

        [Required]
        [Column("supervisorUserId")]
        public int SupervisorUserId { get; set; }

        [Required]
        [Column("status")]
        [StringLength(50)]
        public string Status { get; set; }

        [Column("startedAt")]
        public DateTime? StartedAt { get; set; }

        [Column("endedAt")]
        public DateTime? EndedAt { get; set; }

        [Required]
        [Column("createdAt")]
        public DateTime CreatedAt { get; set; }

        [Required]
        [Column("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("SessionId")]
        public virtual Session Session { get; set; }

        [ForeignKey("SupervisorUserId")]
        public virtual User Supervisor { get; set; }

        public virtual ICollection<ExamParticipant> Participants { get; set; }
        public virtual ICollection<ExamLog> Logs { get; set; }
        public virtual ICollection<ExamMessage> Messages { get; set; }
    }
}
