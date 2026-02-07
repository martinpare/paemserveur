using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("validationInstanceSteps")]
    public class ValidationInstanceStep
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("validationInstanceId")]
        public int ValidationInstanceId { get; set; }

        [Required]
        [Column("validationProcessStepId")]
        public int ValidationProcessStepId { get; set; }

        [Required]
        [Column("statusId")]
        public int StatusId { get; set; }

        [Column("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [Column("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("ValidationInstanceId")]
        public virtual ValidationInstance ValidationInstance { get; set; }

        [ForeignKey("ValidationProcessStepId")]
        public virtual ValidationProcessStep ValidationProcessStep { get; set; }

        [ForeignKey("StatusId")]
        public virtual ValueDomainItem Status { get; set; }

        public virtual ICollection<ValidationInstanceStepValidation> Validations { get; set; }
    }
}
