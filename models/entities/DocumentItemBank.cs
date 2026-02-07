using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("documentItemBanks")]
    public class DocumentItemBank
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("documentId")]
        public int DocumentId { get; set; }

        [Required]
        [Column("itemBankId")]
        public int ItemBankId { get; set; }

        [Column("createdAt")]
        public DateTime? CreatedAt { get; set; }

        // Navigation properties
        [ForeignKey("DocumentId")]
        public virtual Document Document { get; set; }

        [ForeignKey("ItemBankId")]
        public virtual ItemBank ItemBank { get; set; }
    }
}
