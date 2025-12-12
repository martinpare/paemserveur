using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models
{
    [Table("role_permission")]
    public class RolePermission
    {
        [Column("role_id")]
        public long RoleId { get; set; }

        [ForeignKey("RoleId")]
        public Role Role { get; set; }

        [Column("permission_id")]
        public long PermissionId { get; set; }

        [ForeignKey("PermissionId")]
        public Permission Permission { get; set; }

        [Column("accorde_le")]
        public DateTimeOffset AccordeLe { get; set; } = DateTimeOffset.Now;
    }
}
