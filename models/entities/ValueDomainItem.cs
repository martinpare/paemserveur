using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("valueDomainItems")]
    public class ValueDomainItem
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("valueDomainId")]
        public int ValueDomainId { get; set; }

        [Required]
        [Column("order")]
        public int Order { get; set; }

        [Column("code")]
        [StringLength(50)]
        public string Code { get; set; }

        [Required]
        [Column("valueFr")]
        [StringLength(255)]
        public string ValueFr { get; set; }

        [Required]
        [Column("valueEn")]
        [StringLength(255)]
        public string ValueEn { get; set; }

        [Column("descriptionFR")]
        public string DescriptionFr { get; set; }

        [Column("descriptionEn")]
        public string DescriptionEn { get; set; }

        // Navigation properties
        [ForeignKey("ValueDomainId")]
        public virtual ValueDomain ValueDomain { get; set; }
    }
}
