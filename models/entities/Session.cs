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

        [Required]
        [Column("testId")]
        public int TestId { get; set; }

        [Required]
        [Column("scheduledAt")]
        public DateTime ScheduledAt { get; set; }

        [Required]
        [Column("authTypeId")]
        public int AuthTypeId { get; set; }

        [Required]
        [Column("durationMinutes")]
        public int DurationMinutes { get; set; }

        [Column("allowLanguageChange")]
        public bool? AllowLanguageChange { get; set; }

        [Column("allowItemMarking")]
        public bool? AllowItemMarking { get; set; }

        [Column("allowNoteTaking")]
        public bool? AllowNoteTaking { get; set; }

        [Column("allowItemScrambling")]
        public bool? AllowItemScrambling { get; set; }

        [Column("allowHighContrast")]
        public bool? AllowHighContrast { get; set; }

        [Column("forceFullscreen")]
        public bool? ForceFullscreen { get; set; }

        [Column("enableKeyboardShortcuts")]
        public bool? EnableKeyboardShortcuts { get; set; }

        [Column("enableGuidedTour")]
        public bool? EnableGuidedTour { get; set; }

        [Column("questionSummaryNotice")]
        public int? QuestionSummaryNotice { get; set; }

        [Column("enableRemoteMode")]
        public bool? EnableRemoteMode { get; set; }

        [Column("debugMode")]
        public bool? DebugMode { get; set; }
    }
}
