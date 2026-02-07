using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("itemBanks")]
    public class ItemBank
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("nameFr")]
        [StringLength(255)]
        public string NameFr { get; set; }

        [Required]
        [Column("nameEn")]
        [StringLength(255)]
        public string NameEn { get; set; }

        [Column("descriptionFr")]
        [StringLength(1000)]
        public string DescriptionFr { get; set; }

        [Column("descriptionEn")]
        [StringLength(1000)]
        public string DescriptionEn { get; set; }

        [Column("pedagogicalStrudtureId")]
        public int? PedagogicalStructureId { get; set; }

        [Column("isActive")]
        public bool? IsActive { get; set; }

        [Column("hasFeedback")]
        public bool? HasFeedback { get; set; }

        [Column("hasFeedbackForChoices")]
        public bool? HasFeedbackForChoices { get; set; }

        [Column("hasDocumentation")]
        public bool? HasDocumentation { get; set; }

        [Column("hasDocumentationForChoices")]
        public bool? HasDocumentationForChoices { get; set; }

        // Navigation properties
        [ForeignKey("PedagogicalStructureId")]
        public virtual PedagogicalStructure PedagogicalStructure { get; set; }

        public virtual ICollection<DocumentItemBank> DocumentItemBanks { get; set; }
    }
}
