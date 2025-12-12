using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models
{
    [Table("v_role_complet")]
    public class RoleComplet
    {
        [Column("role_id")]
        public long RoleId { get; set; }

        [Column("role_nom")]
        public string RoleNom { get; set; }

        [Column("role_description")]
        public string RoleDescription { get; set; }

        [Column("role_cree_le")]
        public DateTimeOffset RoleCreeLe { get; set; }

        [Column("permission_id")]
        public long? PermissionId { get; set; }

        [Column("permission_code")]
        public string PermissionCode { get; set; }

        [Column("permission_action")]
        public string PermissionAction { get; set; }

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

        [Column("nb_utilisateurs")]
        public int NbUtilisateurs { get; set; }
    }
}
