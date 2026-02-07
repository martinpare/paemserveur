using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("groups")]
    public class Group
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("name")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [Column("learningCenterId")]
        public int LearningCenterId { get; set; }

        [Required]
        [Column("gradeId")]
        public int GradeId { get; set; }

        [Column("teacherId")]
        public int? TeacherId { get; set; }

        [Column("proctorId")]
        public int? ProctorId { get; set; }

        [Required]
        [Column("languageId")]
        public int LanguageId { get; set; }

        [Required]
        [Column("academicYearId")]
        public int AcademicYearId { get; set; }

        [Column("allowManualEntry")]
        public bool AllowManualEntry { get; set; }

        [Column("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [Column("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("LearningCenterId")]
        public virtual LearningCenter LearningCenter { get; set; }

        [ForeignKey("GradeId")]
        public virtual ValueDomainItem Grade { get; set; }

        [ForeignKey("TeacherId")]
        public virtual User Teacher { get; set; }

        [ForeignKey("ProctorId")]
        public virtual User Proctor { get; set; }

        [ForeignKey("AcademicYearId")]
        public virtual ValueDomainItem AcademicYear { get; set; }

        [ForeignKey("LanguageId")]
        public virtual ValueDomainItem Language { get; set; }
    }
}
