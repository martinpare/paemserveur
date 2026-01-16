using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("items")]
    public class Item
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("typeId")]
        public int TypeId { get; set; }

        [Required]
        [Column("label")]
        [StringLength(500)]
        public string Label { get; set; }

        [Required]
        [Column("statusId")]
        public int StatusId { get; set; }

        [Column("userId")]
        public int? UserId { get; set; }

        [Column("documentId")]
        public int? DocumentId { get; set; }

        [Column("itemBankId")]
        public int? ItemBankId { get; set; }

        [Column("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [Column("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [Column("number")]
        public int? Number { get; set; }

        [Column("context")]
        public string Context { get; set; }

        [Required]
        [Column("statement")]
        public string Statement { get; set; }

        [Column("points", TypeName = "decimal(10,2)")]
        public decimal? Points { get; set; }

        [Column("required")]
        public bool? Required { get; set; }

        [Column("hint")]
        public string Hint { get; set; }

        [Column("feedback")]
        public string Feedback { get; set; }

        [Column("media")]
        public string Media { get; set; }

        [Column("data")]
        public string Data { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("DocumentId")]
        public virtual Document Document { get; set; }

        [ForeignKey("ItemBankId")]
        public virtual ItemBank ItemBank { get; set; }
    }
}
