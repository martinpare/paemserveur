using System;

namespace serveur.Models
{
    /// <summary>
    /// Résultat de la procédure sp_obtenir_permissions_role
    /// </summary>
    public class PermissionRoleInfo
    {
        public long PermissionId { get; set; }
        public string PermissionCode { get; set; }
        public string Action { get; set; }
        public string PermissionDescription { get; set; }
        public long? RessourceId { get; set; }
        public string RessourceCode { get; set; }
        public string RessourceType { get; set; }
        public string RessourceNom { get; set; }
        public DateTimeOffset AccordeLe { get; set; }
    }
}
