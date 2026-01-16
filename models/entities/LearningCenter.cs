using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("learningCenters")]
    public class LearningCenter
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("code")]
        [StringLength(10)]
        public string Code { get; set; }

        [Column("internalCode")]
        [StringLength(50)]
        public string InternalCode { get; set; }

        [Required]
        [Column("shortName")]
        [StringLength(100)]
        public string ShortName { get; set; }

        [Column("officialName")]
        [StringLength(255)]
        public string OfficialName { get; set; }

        [Column("email")]
        [StringLength(255)]
        public string Email { get; set; }

        [Column("website")]
        [StringLength(255)]
        public string Website { get; set; }

        [Column("phone")]
        [StringLength(20)]
        public string Phone { get; set; }

        [Column("phoneExtension")]
        [StringLength(10)]
        public string PhoneExtension { get; set; }

        [Column("address")]
        [StringLength(255)]
        public string Address { get; set; }

        [Column("city")]
        [StringLength(100)]
        public string City { get; set; }

        [Column("province")]
        public int? Province { get; set; }

        [Column("postalCode")]
        [StringLength(10)]
        public string PostalCode { get; set; }

        [Column("administrativeRegion")]
        public int? AdministrativeRegion { get; set; }

        [Column("educationNetwork")]
        public int? EducationNetwork { get; set; }

        [Column("educationLevel")]
        public string EducationLevel { get; set; }

        [Column("teachingLanguage")]
        public int? TeachingLanguage { get; set; }

        [Column("organisationId")]
        public int? OrganisationId { get; set; }

        // Navigation properties
        [ForeignKey("OrganisationId")]
        public virtual Organisation Organisation { get; set; }
    }
}
