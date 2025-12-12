using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models
{
    /// <summary>
    /// Modèle pour la vue v_permission_sans_ressource
    /// Représente une permission non liée à une ressource
    /// </summary>
    [Table("v_permission_sans_ressource")]
    public class PermissionSansRessource
    {
        [Column("permission_id")]
        public long PermissionId { get; set; }

        [Column("code")]
        public string Code { get; set; }

        [Column("action")]
        public string Action { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("cree_le")]
        public DateTimeOffset CreeLe { get; set; }

        [Column("nb_roles")]
        public int NbRoles { get; set; }
    }
}
