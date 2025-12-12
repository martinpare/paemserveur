using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models
{
    /// <summary>
    /// Modèle pour la vue v_permission_role_utilisateur
    /// Représente une permission avec son rôle et utilisateur associé
    /// </summary>
    [Table("v_permission_role_utilisateur")]
    public class PermissionRoleUtilisateur
    {
        [Column("permission_id")]
        public long PermissionId { get; set; }

        [Column("permission_code")]
        public string PermissionCode { get; set; }

        [Column("action")]
        public string Action { get; set; }

        [Column("ressource_id")]
        public long? RessourceId { get; set; }

        [Column("ressource_code")]
        public string RessourceCode { get; set; }

        [Column("ressource_type")]
        public string RessourceType { get; set; }

        [Column("role_id")]
        public long RoleId { get; set; }

        [Column("role_nom")]
        public string RoleNom { get; set; }

        [Column("utilisateur_id")]
        public long UtilisateurId { get; set; }

        [Column("utilisateur_nom")]
        public string UtilisateurNom { get; set; }

        [Column("utilisateur_prenom")]
        public string UtilisateurPrenom { get; set; }

        [Column("utilisateur_courriel")]
        public string UtilisateurCourriel { get; set; }

        [Column("utilisateur_nom_utilisateur")]
        public string UtilisateurNomUtilisateur { get; set; }
    }
}
