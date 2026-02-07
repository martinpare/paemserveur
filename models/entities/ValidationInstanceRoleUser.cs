using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("validationInstanceRoleUsers")]
    public class ValidationInstanceRoleUser
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("validationInstanceRoleId")]
        public int ValidationInstanceRoleId { get; set; }

        [Required]
        [Column("userId")]
        public int UserId { get; set; }

        [Column("createdAt")]
        public DateTime? CreatedAt { get; set; }

        // Navigation properties
        [ForeignKey("ValidationInstanceRoleId")]
        public virtual ValidationInstanceRole ValidationInstanceRole { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
