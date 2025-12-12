using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models
{
    /// <summary>
    /// Modèle pour la vue v_ressource_utilisateur
    /// Représente une ressource accessible par un utilisateur avec l'action autorisée
    /// </summary>
    [Table("v_ressource_utilisateur")]
    public class RessourceUtilisateur
    {
        [Column("utilisateur_id")]
        public long UtilisateurId { get; set; }

        [Column("ressource_id")]
        public long RessourceId { get; set; }

        [Column("ressource_type")]
        public string RessourceType { get; set; }

        [Column("ressource_code")]
        public string RessourceCode { get; set; }

        [Column("ressource_nom")]
        public string RessourceNom { get; set; }

        [Column("action")]
        public string Action { get; set; }

        [Column("permission_code")]
        public string PermissionCode { get; set; }
    }
}
