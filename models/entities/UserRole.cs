using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("userRoles")]
    public class UserRole
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("userId")]
        public int UserId { get; set; }

        [Required]
        [Column("roleId")]
        public int RoleId { get; set; }

        [Column("organisationId")]
        public int? OrganisationId { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }

        [ForeignKey("OrganisationId")]
        public virtual Organisation Organisation { get; set; }
    }
}
