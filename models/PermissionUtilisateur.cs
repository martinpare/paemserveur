using System;

namespace serveur.Models
{
    public class PermissionUtilisateur
    {
        // Colonnes retournées par sp_obtenir_utilisateur_permissions (3ème résultat)
        public long Id { get; set; }
        public string Code { get; set; }
        public string Action { get; set; }
        public string Description { get; set; }
        public long? Ressource_Id { get; set; }
        public string Ressource_Code { get; set; }
        public string Ressource_Type { get; set; }
        public string Ressource_Nom { get; set; }
    }
}
