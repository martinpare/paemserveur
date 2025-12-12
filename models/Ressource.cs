using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models
{
    [Table("ressource")]
    public class Ressource
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("type")]
        [StringLength(100)]
        public string Type { get; set; }

        [Required]
        [Column("code")]
        [StringLength(150)]
        public string Code { get; set; }

        [Column("nom")]
        [StringLength(255)]
        public string Nom { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("cree_le")]
        public DateTimeOffset CreeLe { get; set; } = DateTimeOffset.Now;
    }
}
