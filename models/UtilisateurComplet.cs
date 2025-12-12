using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models
{
    [Table("v_utilisateur_complet")]
    public class UtilisateurComplet
    {
        [Column("utilisateur_id")]
        public long Id { get; set; }

        [Column("nom_utilisateur")]
        public string NomUtilisateur { get; set; }

        [Column("courriel")]
        public string Courriel { get; set; }

        [Column("prenom")]
        public string Prenom { get; set; }

        [Column("nom")]
        public string Nom { get; set; }

        [Column("est_actif")]
        public bool EstActif { get; set; }

        [Column("utilisateur_cree_le")]
        public DateTimeOffset CreeLe { get; set; }

        [Column("role_id")]
        public long? RoleId { get; set; }

        [Column("role_nom")]
        public string NomRole { get; set; }

        [Column("role_description")]
        public string DescriptionRole { get; set; }

        [Column("role_assigne_le")]
        public DateTimeOffset? RoleAssigneLe { get; set; }

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
    }
}
