using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("validationInstances")]
    public class ValidationInstance
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("validationProcessId")]
        public int ValidationProcessId { get; set; }

        [Required]
        [Column("reviewableTypeId")]
        public int ReviewableTypeId { get; set; }

        [Required]
        [Column("targetEntityId")]
        public int TargetEntityId { get; set; }

        [Column("currentStepId")]
        public int? CurrentStepId { get; set; }

        [Required]
        [Column("statusId")]
        public int StatusId { get; set; }

        [Required]
        [Column("initiatedByUserId")]
        public int InitiatedByUserId { get; set; }

        [Required]
        [Column("initiatedAt")]
        public DateTime InitiatedAt { get; set; }

        [Column("completedAt")]
        public DateTime? CompletedAt { get; set; }

        [Column("data")]
        public string Data { get; set; }

        [Column("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [Column("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("ValidationProcessId")]
        public virtual ValidationProcess ValidationProcess { get; set; }

        [ForeignKey("ReviewableTypeId")]
        public virtual ValueDomainItem ReviewableType { get; set; }

        [ForeignKey("CurrentStepId")]
        public virtual ValidationInstanceStep CurrentStep { get; set; }

        [ForeignKey("StatusId")]
        public virtual ValueDomainItem Status { get; set; }

        [ForeignKey("InitiatedByUserId")]
        public virtual User InitiatedByUser { get; set; }

        public virtual ICollection<ValidationInstanceStep> Steps { get; set; }
        public virtual ICollection<ValidationInstanceRole> Roles { get; set; }
    }
}
