using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models
{
    [Table("v_ressource_permissions")]
    public class RessourcePermissions
    {
        [Column("ressource_id")]
        public long RessourceId { get; set; }

        [Column("ressource_code")]
        public string RessourceCode { get; set; }

        [Column("ressource_type")]
        public string RessourceType { get; set; }

        [Column("ressource_nom")]
        public string RessourceNom { get; set; }

        [Column("ressource_description")]
        public string RessourceDescription { get; set; }

        [Column("permission_id")]
        public long? PermissionId { get; set; }

        [Column("permission_code")]
        public string PermissionCode { get; set; }

        [Column("action")]
        public string Action { get; set; }

        [Column("permission_description")]
        public string PermissionDescription { get; set; }

        [Column("nb_roles")]
        public int NbRoles { get; set; }
    }
}
