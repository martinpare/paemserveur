using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("pedagogicalElementTypes")]
    public class PedagogicalElementType
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("code")]
        [StringLength(50)]
        public string Code { get; set; }

        [Column("descriptionFr")]
        [StringLength(255)]
        public string DescriptionFr { get; set; }

        [Column("descriptionEn")]
        [StringLength(255)]
        public string DescriptionEn { get; set; }

        [Column("organisationId")]
        public int? OrganisationId { get; set; }

        // Navigation properties
        [ForeignKey("OrganisationId")]
        public virtual Organisation Organisation { get; set; }
    }
}
