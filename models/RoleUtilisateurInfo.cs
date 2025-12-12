using System;

namespace serveur.Models
{
    /// <summary>
    /// Résultat de la procédure sp_obtenir_roles_utilisateur
    /// </summary>
    public class RoleUtilisateurInfo
    {
        public long RoleId { get; set; }
        public string RoleNom { get; set; }
        public string RoleDescription { get; set; }
        public DateTimeOffset AssigneLe { get; set; }
        public string TypeAttribution { get; set; }
        public string TypePortee { get; set; }
        public long? PorteeId { get; set; }
    }
}
