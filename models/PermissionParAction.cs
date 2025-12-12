using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models
{
    /// <summary>
    /// Modèle pour la vue v_permission_par_action
    /// Représente un regroupement de permissions par action
    /// </summary>
    [Table("v_permission_par_action")]
    public class PermissionParAction
    {
        [Column("action")]
        public string Action { get; set; }

        [Column("nb_permissions")]
        public int NbPermissions { get; set; }

        [Column("nb_ressources_distinctes")]
        public int NbRessourcesDistinctes { get; set; }

        [Column("codes")]
        public string Codes { get; set; }
    }
}
