using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models
{
    /// <summary>
    /// DTO pour les résultats des procédures stockées de ressources
    /// Utilisé par sp_obtenir_ressource_complete et sp_rechercher_ressources
    /// </summary>
    public class RessourceAvecStats
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
        public int? NbUtilisateursAyantAcces { get; set; }

        [Column("est_utilisee")]
        public bool EstUtilisee { get; set; }
    }

    /// <summary>
    /// DTO pour les résultats de la fonction fn_obtenir_ressources_utilisateur
    /// </summary>
    public class RessourceUtilisateurDto
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

        [Column("action")]
        public string Action { get; set; }
    }

    /// <summary>
    /// DTO pour les paramètres de recherche de ressources
    /// </summary>
    public class RessourceRechercheParams
    {
        public string Type { get; set; }
        public string CodeContient { get; set; }
        public string NomContient { get; set; }
        public bool UniquementUtilisees { get; set; } = false;
    }

    /// <summary>
    /// DTO pour la création d'une ressource
    /// </summary>
    public class RessourceCreationDto
    {
        public string Type { get; set; }
        public string Code { get; set; }
        public string Nom { get; set; }
        public string Description { get; set; }
    }

    /// <summary>
    /// DTO pour la duplication d'une ressource
    /// </summary>
    public class RessourceDuplicationDto
    {
        public string NouveauCode { get; set; }
        public string NouveauNom { get; set; }
    }
}
