using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("examIncidents")]
    public class ExamIncident
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("examParticipantId")]
        public int ExamParticipantId { get; set; }

        [Required]
        [Column("incidentType")]
        [StringLength(50)]
        public string IncidentType { get; set; }

        [Column("description")]
        [StringLength(500)]
        public string Description { get; set; }

        [Column("previousStatus")]
        [StringLength(50)]
        public string PreviousStatus { get; set; }

        [Required]
        [Column("createdAt")]
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        [ForeignKey("ExamParticipantId")]
        public virtual ExamParticipant ExamParticipant { get; set; }
    }
}
