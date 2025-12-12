using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models
{
    [Table("v_permission_orpheline")]
    public class PermissionOrpheline
    {
        [Column("permission_id")]
        public long PermissionId { get; set; }

        [Column("permission_code")]
        public string PermissionCode { get; set; }

        [Column("action")]
        public string Action { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("ressource_code")]
        public string RessourceCode { get; set; }

        [Column("ressource_type")]
        public string RessourceType { get; set; }

        [Column("cree_le")]
        public DateTimeOffset CreeLe { get; set; }
    }
}
