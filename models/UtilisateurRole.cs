using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models
{
    [Table("utilisateur_role")]
    public class UtilisateurRole
    {
        [Column("utilisateur_id")]
        public long UtilisateurId { get; set; }

        [ForeignKey("UtilisateurId")]
        public Utilisateur Utilisateur { get; set; }

        [Column("role_id")]
        public long RoleId { get; set; }

        [ForeignKey("RoleId")]
        public Role Role { get; set; }

        [Column("assigne_le")]
        public DateTimeOffset AssigneLe { get; set; } = DateTimeOffset.Now;
    }
}
