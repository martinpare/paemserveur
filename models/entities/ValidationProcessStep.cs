using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("validationProcessSteps")]
    public class ValidationProcessStep
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("validationProcessId")]
        public int ValidationProcessId { get; set; }

        [Required]
        [Column("roleId")]
        public int RoleId { get; set; }

        [Required]
        [Column("stepOrder")]
        public int StepOrder { get; set; }

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

        [Required]
        [Column("isMandatory")]
        public bool IsMandatory { get; set; }

        [Required]
        [Column("requiredValidations")]
        public int RequiredValidations { get; set; }

        [Column("timeoutDays")]
        public int? TimeoutDays { get; set; }

        [Column("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [Column("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("ValidationProcessId")]
        public virtual ValidationProcess ValidationProcess { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }

        public virtual ICollection<ValidationInstanceStep> InstanceSteps { get; set; }
    }
}
