using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("organisations")]
    public class Organisation
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("nameFr")]
        [StringLength(255)]
        public string NameFr { get; set; }

        [Column("nameEn")]
        [StringLength(255)]
        public string NameEn { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Required]
        [Column("isActive")]
        public bool IsActive { get; set; }

        [Column("acronym")]
        [StringLength(50)]
        public string Acronym { get; set; }

        [Column("address")]
        [StringLength(255)]
        public string Address { get; set; }

        [Column("city")]
        [StringLength(100)]
        public string City { get; set; }
    }
}
