using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models
{
    [Table("utilisateur")]
    public class Utilisateur
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("nom_utilisateur")]
        [StringLength(100)]
        public string NomUtilisateur { get; set; }

        [Required]
        [Column("courriel")]
        [StringLength(255)]
        public string Courriel { get; set; }

        [Required]
        [Column("mot_de_passe_hash")]
        public string MotDePasseHash { get; set; }

        [Column("est_actif")]
        public bool EstActif { get; set; } = true;

        [Column("cree_le")]
        public DateTimeOffset CreeLe { get; set; } = DateTimeOffset.Now;

        [Required]
        [Column("prenom")]
        [StringLength(255)]
        public string Prenom { get; set; }

        [Required]
        [Column("nom")]
        [StringLength(255)]
        public string Nom { get; set; }
    }
}
