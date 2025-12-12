using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models
{
    [Table("role")]
    public class Role
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("nom")]
        [StringLength(100)]
        public string Nom { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("cree_le")]
        public DateTimeOffset CreeLe { get; set; } = DateTimeOffset.Now;
    }
}
