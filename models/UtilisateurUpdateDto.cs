using System.ComponentModel.DataAnnotations;

namespace serveur.Models
{
    public class UtilisateurUpdateDto
    {
        [Required]
        [StringLength(100)]
        public string NomUtilisateur { get; set; }

        [Required]
        [StringLength(255)]
        public string Courriel { get; set; }

        [Required]
        [StringLength(255)]
        public string Prenom { get; set; }

        [Required]
        [StringLength(255)]
        public string Nom { get; set; }

        public bool EstActif { get; set; }

        // Optionnel - si vide, le mot de passe n'est pas modifié
        public string MotDePasseHash { get; set; }
    }
}
