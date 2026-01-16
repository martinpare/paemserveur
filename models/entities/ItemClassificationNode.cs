using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("itemClassificationNodes")]
    public class ItemClassificationNode
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("itemId")]
        public int ItemId { get; set; }

        [Required]
        [Column("classificationNodeId")]
        public int ClassificationNodeId { get; set; }

        [Column("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [Column("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("ItemId")]
        public virtual Item Item { get; set; }

        [ForeignKey("ClassificationNodeId")]
        public virtual ClassificationNode ClassificationNode { get; set; }
    }
}
