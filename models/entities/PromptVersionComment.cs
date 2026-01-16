using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("promptVersionComments")]
    public class PromptVersionComment
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("promptVersionId")]
        public int PromptVersionId { get; set; }

        [Required]
        [Column("userId")]
        public int UserId { get; set; }

        [Required]
        [Column("content")]
        public string Content { get; set; }

        [Required]
        [Column("createdAt")]
        public DateTime CreatedAt { get; set; }

        [Required]
        [Column("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("PromptVersionId")]
        public virtual PromptVersion PromptVersion { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
