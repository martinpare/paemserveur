using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models
{
    [Table("permission")]
    public class Permission
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("code")]
        [StringLength(150)]
        public string Code { get; set; }

        [Required]
        [Column("action")]
        [StringLength(50)]
        public string Action { get; set; }

        [Column("ressource_id")]
        public long? RessourceId { get; set; }

        [ForeignKey("RessourceId")]
        public Ressource Ressource { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("cree_le")]
        public DateTimeOffset CreeLe { get; set; } = DateTimeOffset.Now;
    }
}
