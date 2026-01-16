using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("administrationCenters")]
    public class AdministrationCenter
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("organisationId")]
        public int? OrganisationId { get; set; }

        [Required]
        [Column("code")]
        [StringLength(10)]
        public string Code { get; set; }

        [Required]
        [Column("shortName")]
        [StringLength(100)]
        public string ShortName { get; set; }

        [Column("officialName")]
        [StringLength(255)]
        public string OfficialName { get; set; }

        [Column("address")]
        [StringLength(255)]
        public string Address { get; set; }

        [Column("city")]
        [StringLength(100)]
        public string City { get; set; }

        [Column("province")]
        public int? Province { get; set; }

        [Column("country")]
        public int? Country { get; set; }

        [Column("postalCode")]
        [StringLength(10)]
        public string PostalCode { get; set; }

        [Column("email")]
        [StringLength(255)]
        public string Email { get; set; }

        [Column("contactPhone")]
        [StringLength(20)]
        public string ContactPhone { get; set; }

        [Column("contactExtension")]
        [StringLength(10)]
        public string ContactExtension { get; set; }

        // Navigation properties
        [ForeignKey("OrganisationId")]
        public virtual Organisation Organisation { get; set; }
    }
}
