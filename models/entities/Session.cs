using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("sessions")]
    public class Session
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("pedagogicalStructureId")]
        public int? PedagogicalStructureId { get; set; }

        [Required]
        [Column("nameFr")]
        [StringLength(255)]
        public string NameFr { get; set; }

        [Column("nameEn")]
        [StringLength(255)]
        public string NameEn { get; set; }

        [Column("examId")]
        public int? ExamId { get; set; }

        [Column("scheduledFrom")]
        public DateTime? ScheduledFrom { get; set; }

        [Column("scheduledTo")]
        public DateTime? ScheduledTo { get; set; }

        [Column("timeStart")]
        public TimeSpan? TimeStart { get; set; }

        [Column("timeEnd")]
        public TimeSpan? TimeEnd { get; set; }

        [Column("examPeriodId")]
        public int? ExamPeriodId { get; set; }

        // Navigation properties
        [ForeignKey("ExamId")]
        public virtual Exam Exam { get; set; }

        [ForeignKey("ExamPeriodId")]
        public virtual ExamPeriod ExamPeriod { get; set; }

        [ForeignKey("PedagogicalStructureId")]
        public virtual PedagogicalStructure PedagogicalStructure { get; set; }
    }
}
