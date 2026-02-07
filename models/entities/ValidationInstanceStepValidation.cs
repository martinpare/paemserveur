using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("validationInstanceStepValidations")]
    public class ValidationInstanceStepValidation
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("validationInstanceStepId")]
        public int ValidationInstanceStepId { get; set; }

        [Required]
        [Column("validatedByUserId")]
        public int ValidatedByUserId { get; set; }

        [Required]
        [Column("decision")]
        [StringLength(20)]
        public string Decision { get; set; }

        [Required]
        [Column("validatedAt")]
        public DateTime ValidatedAt { get; set; }

        [Column("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [Column("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("ValidationInstanceStepId")]
        public virtual ValidationInstanceStep ValidationInstanceStep { get; set; }

        [ForeignKey("ValidatedByUserId")]
        public virtual User ValidatedByUser { get; set; }
    }
}
