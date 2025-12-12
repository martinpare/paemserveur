using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models
{
    /// <summary>
    /// Modèle pour la vue v_ressource_par_type
    /// Représente un regroupement de ressources par type
    /// </summary>
    [Table("v_ressource_par_type")]
    public class RessourceParType
    {
        [Column("type")]
        public string Type { get; set; }

        [Column("nb_ressources")]
        public int NbRessources { get; set; }

        [Column("codes")]
        public string Codes { get; set; }
    }
}
