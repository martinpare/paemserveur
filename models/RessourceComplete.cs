using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models
{
    /// <summary>
    /// Modèle pour la vue v_ressource_complete
    /// Représente une ressource avec ses statistiques d'utilisation
    /// </summary>
    [Table("v_ressource_complete")]
    public class RessourceComplete
    {
        [Column("ressource_id")]
        public long RessourceId { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("code")]
        public string Code { get; set; }

        [Column("nom")]
        public string Nom { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("cree_le")]
        public DateTimeOffset CreeLe { get; set; }

        [Column("nb_permissions")]
        public int NbPermissions { get; set; }

        [Column("nb_roles_utilisant")]
        public int NbRolesUtilisant { get; set; }

        [Column("nb_utilisateurs_ayant_acces")]
        public int NbUtilisateursAyantAcces { get; set; }
    }
}
