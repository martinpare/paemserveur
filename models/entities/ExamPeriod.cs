using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("examPeriods")]
    public class ExamPeriod
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("nameFr")]
        [StringLength(100)]
        public string NameFr { get; set; }

        [Required]
        [Column("nameEn")]
        [StringLength(100)]
        public string NameEn { get; set; }

        [Required]
        [Column("year")]
        public int Year { get; set; }

        [Required]
        [Column("month")]
        public int Month { get; set; }

        [Required]
        [Column("startDate")]
        public DateTime StartDate { get; set; }

        [Required]
        [Column("endDate")]
        public DateTime EndDate { get; set; }

        [Column("isActive")]
        public bool IsActive { get; set; } = true;

        [Column("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [Column("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
    }
}
