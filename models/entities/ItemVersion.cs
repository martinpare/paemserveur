using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("itemVersions")]
    public class ItemVersion
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("itemId")]
        public int ItemId { get; set; }

        [Required]
        [Column("version")]
        public int Version { get; set; }

        [Column("data")]
        public string Data { get; set; }

        [Column("date")]
        public DateTime? Date { get; set; }

        [Column("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [Column("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("ItemId")]
        public virtual Item Item { get; set; }
    }
}
