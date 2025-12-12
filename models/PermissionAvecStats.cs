using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models
{
    /// <summary>
    /// DTO pour les résultats des procédures stockées de permissions
    /// Utilisé par sp_obtenir_permission_complete et sp_rechercher_permissions
    /// </summary>
    public class PermissionAvecStats
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

        [Column("est_utilisee")]
        public bool EstUtilisee { get; set; }
    }

    /// <summary>
    /// DTO pour les paramètres de recherche de permissions
    /// </summary>
    public class PermissionRechercheParams
    {
        public string Action { get; set; }
        public string RessourceType { get; set; }
        public string CodeContient { get; set; }
        public bool UniquementUtilisees { get; set; } = false;
        public bool UniquementOrphelines { get; set; } = false;
    }

    /// <summary>
    /// DTO pour la création d'une permission (existant, complété)
    /// </summary>
    public class CreerPermissionDto
    {
        public string Code { get; set; }
        public string Action { get; set; }
        public long? RessourceId { get; set; }
        public string Description { get; set; }
    }

    /// <summary>
    /// DTO pour la modification d'une permission
    /// </summary>
    public class ModifierPermissionDto
    {
        public string Code { get; set; }
        public string Action { get; set; }
        public long? RessourceId { get; set; }
        public string Description { get; set; }
    }

    /// <summary>
    /// DTO pour la duplication d'une permission
    /// </summary>
    public class PermissionDuplicationDto
    {
        public string NouveauCode { get; set; }
        public long? NouvelleRessourceId { get; set; }
    }

    /// <summary>
    /// Résultat de la création d'une permission (existant)
    /// </summary>
    public class CreerPermissionResult
    {
        public long PermissionId { get; set; }
        public string Code { get; set; }
        public string Action { get; set; }
        public long? RessourceId { get; set; }
    }

    /// <summary>
    /// Résultat de la création des permissions CRUD (existant)
    /// </summary>
    public class CreerPermissionsCrudResult
    {
        public long PermissionId { get; set; }
        public string Code { get; set; }
        public string Action { get; set; }
        public string Statut { get; set; }
    }

    /// <summary>
    /// DTO pour les rôles ayant une permission
    /// </summary>
    public class RoleAvecPermissionDto
    {
        [Column("role_id")]
        public long RoleId { get; set; }

        [Column("role_nom")]
        public string RoleNom { get; set; }

        [Column("role_description")]
        public string RoleDescription { get; set; }

        [Column("accorde_le")]
        public DateTimeOffset AccordeLe { get; set; }

        [Column("nb_utilisateurs")]
        public int NbUtilisateurs { get; set; }
    }

    /// <summary>
    /// DTO pour les utilisateurs ayant une permission
    /// </summary>
    public class UtilisateurAvecPermissionDto
    {
        [Column("utilisateur_id")]
        public long UtilisateurId { get; set; }

        [Column("nom")]
        public string Nom { get; set; }

        [Column("prenom")]
        public string Prenom { get; set; }

        [Column("courriel")]
        public string Courriel { get; set; }

        [Column("nom_utilisateur")]
        public string NomUtilisateur { get; set; }

        [Column("est_actif")]
        public bool EstActif { get; set; }

        [Column("role_id")]
        public long RoleId { get; set; }

        [Column("role_nom")]
        public string RoleNom { get; set; }
    }

    /// <summary>
    /// DTO pour l'assignation d'une permission à un rôle
    /// </summary>
    public class AssignerPermissionRoleDto
    {
        public long PermissionId { get; set; }
        public long RoleId { get; set; }
    }

    /// <summary>
    /// DTO pour la copie de permissions entre rôles
    /// </summary>
    public class CopierPermissionsDto
    {
        public long RoleSourceId { get; set; }
        public long RoleCibleId { get; set; }
        public bool Remplacer { get; set; } = false;
    }
}
