using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models
{
    /// <summary>
    /// Modèle pour la vue v_permission_complete
    /// Représente une permission avec ses statistiques d'utilisation
    /// </summary>
    [Table("v_permission_complete")]
    public class PermissionComplete
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

        [Column("ressource_id")]
        public long? RessourceId { get; set; }

        [Column("ressource_type")]
        public string RessourceType { get; set; }

        [Column("ressource_code")]
        public string RessourceCode { get; set; }

        [Column("ressource_nom")]
        public string RessourceNom { get; set; }

        [Column("nb_roles")]
        public int NbRoles { get; set; }

        [Column("nb_utilisateurs")]
        public int NbUtilisateurs { get; set; }
    }
}
