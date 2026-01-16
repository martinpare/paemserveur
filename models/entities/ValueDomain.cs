using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("valueDomains")]
    public class ValueDomain
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

        [Required]
        [Column("tag")]
        [StringLength(100)]
        public string Tag { get; set; }

        [Required]
        [Column("isOrdered")]
        public bool IsOrdered { get; set; }

        [Required]
        [Column("isPublic")]
        public bool IsPublic { get; set; }

        [Column("descriptionFr")]
        public string DescriptionFr { get; set; }

        [Column("descriptionEn")]
        public string DescriptionEn { get; set; }
    }
}
