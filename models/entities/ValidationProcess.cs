using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("validationProcesses")]
    public class ValidationProcess
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("code")]
        [StringLength(50)]
        public string Code { get; set; }

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
        [Column("reviewableTypeId")]
        public int ReviewableTypeId { get; set; }

        [Column("organisationId")]
        public int? OrganisationId { get; set; }

        [Column("pedagogicalStructureId")]
        public int? PedagogicalStructureId { get; set; }

        [Required]
        [Column("isActive")]
        public bool IsActive { get; set; }

        [Column("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [Column("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("ReviewableTypeId")]
        public virtual ValueDomainItem ReviewableType { get; set; }

        [ForeignKey("OrganisationId")]
        public virtual Organisation Organisation { get; set; }

        [ForeignKey("PedagogicalStructureId")]
        public virtual PedagogicalStructure PedagogicalStructure { get; set; }

        public virtual ICollection<ValidationProcessStep> Steps { get; set; }
        public virtual ICollection<ValidationInstance> Instances { get; set; }
    }
}
