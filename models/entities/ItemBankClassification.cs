using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("itemBankClassifications")]
    public class ItemBankClassification
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("itemBankId")]
        public int ItemBankId { get; set; }

        [Required]
        [Column("classificationId")]
        public int ClassificationId { get; set; }

        [Column("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [Column("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("ItemBankId")]
        public virtual ItemBank ItemBank { get; set; }

        [ForeignKey("ClassificationId")]
        public virtual Classification Classification { get; set; }
    }
}
