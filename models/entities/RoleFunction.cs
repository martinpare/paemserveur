using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("roleFunctions")]
    public class RoleFunction
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("roleId")]
        public int RoleId { get; set; }

        [Required]
        [Column("functionCode")]
        [StringLength(50)]
        public string FunctionCode { get; set; }

        // Navigation properties
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
    }
}
