using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models
{
    [Table("v_permission_usage")]
    public class PermissionUsage
    {
        [Column("permission_id")]
        public long PermissionId { get; set; }

        [Column("permission_code")]
        public string PermissionCode { get; set; }

        [Column("action")]
        public string Action { get; set; }

        [Column("permission_description")]
        public string PermissionDescription { get; set; }

        [Column("ressource_id")]
        public long? RessourceId { get; set; }

        [Column("ressource_code")]
        public string RessourceCode { get; set; }

        [Column("ressource_type")]
        public string RessourceType { get; set; }

        [Column("ressource_nom")]
        public string RessourceNom { get; set; }

        [Column("role_id")]
        public long? RoleId { get; set; }

        [Column("role_nom")]
        public string RoleNom { get; set; }

        [Column("accorde_le")]
        public DateTimeOffset? AccordeLe { get; set; }

        [Column("nb_utilisateurs_via_role")]
        public int NbUtilisateursViaRole { get; set; }
    }
}
