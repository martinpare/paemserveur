using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("validationInstanceRoles")]
    public class ValidationInstanceRole
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("validationInstanceId")]
        public int ValidationInstanceId { get; set; }

        [Required]
        [Column("roleId")]
        public int RoleId { get; set; }

        [Column("createdAt")]
        public DateTime? CreatedAt { get; set; }

        // Navigation properties
        [ForeignKey("ValidationInstanceId")]
        public virtual ValidationInstance ValidationInstance { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }

        public virtual ICollection<ValidationInstanceRoleUser> Users { get; set; }
    }
}
