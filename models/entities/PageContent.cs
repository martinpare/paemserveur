using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("pageContents")]
    public class PageContent
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("pedagogicalStrudtureId")]
        public int? PedagogicalStructureId { get; set; }

        [Required]
        [Column("nameFr")]
        [StringLength(255)]
        public string NameFr { get; set; }

        [Column("nameEn")]
        [StringLength(255)]
        public string NameEn { get; set; }

        [Column("descriptionFr")]
        public string DescriptionFr { get; set; }

        [Column("descriptionEn")]
        public string DescriptionEn { get; set; }

        [Required]
        [Column("contentFr")]
        public string ContentFr { get; set; }

        [Column("contentEn")]
        public string ContentEn { get; set; }

        [Required]
        [Column("isEndPage")]
        public bool IsEndPage { get; set; }

        [Required]
        [Column("requiresConsent")]
        public bool RequiresConsent { get; set; }

        [Column("consentTextFr")]
        public string ConsentTextFr { get; set; }

        [Column("consentTextEn")]
        public string ConsentTextEn { get; set; }
    }
}
