using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models
{
    [Table("attribution_role_portee")]
    public class AttributionRolePortee
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("utilisateur_id")]
        public long UtilisateurId { get; set; }

        [ForeignKey("UtilisateurId")]
        public Utilisateur Utilisateur { get; set; }

        [Column("role_id")]
        public long RoleId { get; set; }

        [ForeignKey("RoleId")]
        public Role Role { get; set; }

        [Required]
        [Column("type_portee")]
        [StringLength(100)]
        public string TypePortee { get; set; }

        [Column("portee_id")]
        public long PorteeId { get; set; }

        [Column("assigne_le")]
        public DateTimeOffset AssigneLe { get; set; } = DateTimeOffset.Now;
    }
}
