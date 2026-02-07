using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("exams")]
    public class Exam
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

        [Required]
        [Column("nameEn")]
        [StringLength(255)]
        public string NameEn { get; set; }

        [Column("descriptionFr")]
        public string DescriptionFr { get; set; }

        [Column("descriptionEn")]
        public string DescriptionEn { get; set; }

        [Column("externalCode")]
        [StringLength(100)]
        public string ExternalCode { get; set; }

        [Column("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [Column("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [Column("documentId")]
        public int? DocumentId { get; set; }

        [Column("examPeriodId")]
        public int? ExamPeriodId { get; set; }

        [Column("competencyId")]
        public int? CompetencyId { get; set; }

        [Column("parentExamId")]
        public int? ParentExamId { get; set; }

        [Column("partNumber")]
        public int? PartNumber { get; set; }

        [Column("partNameFr")]
        [StringLength(100)]
        public string PartNameFr { get; set; }

        [Column("partNameEn")]
        [StringLength(100)]
        public string PartNameEn { get; set; }

        [Column("languageId")]
        public int? LanguageId { get; set; }

        // Navigation properties
        [ForeignKey("PedagogicalStructureId")]
        public virtual PedagogicalStructure PedagogicalStructure { get; set; }

        [ForeignKey("DocumentId")]
        public virtual Document Document { get; set; }

        [ForeignKey("ExamPeriodId")]
        public virtual ExamPeriod ExamPeriod { get; set; }

        [ForeignKey("CompetencyId")]
        public virtual ValueDomainItem Competency { get; set; }

        [ForeignKey("ParentExamId")]
        public virtual Exam ParentExam { get; set; }

        [ForeignKey("LanguageId")]
        public virtual ValueDomainItem Language { get; set; }
    }
}
