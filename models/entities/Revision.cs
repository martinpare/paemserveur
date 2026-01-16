using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("revisions")]
    public class Revision
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("itemId")]
        public int? ItemId { get; set; }

        [Column("date")]
        public DateTime? Date { get; set; }

        [Column("data")]
        public string Data { get; set; }

        [Column("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [Column("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("ItemId")]
        public virtual Item Item { get; set; }
    }
}
