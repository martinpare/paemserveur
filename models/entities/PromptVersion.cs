using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("promptVersions")]
    public class PromptVersion
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("promptId")]
        public int PromptId { get; set; }

        [Required]
        [Column("version")]
        [StringLength(50)]
        public string Version { get; set; }

        [Column("newContent")]
        public string NewContent { get; set; }

        [Column("active")]
        public bool? Active { get; set; }

        [Column("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [Column("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("PromptId")]
        public virtual Prompt Prompt { get; set; }

        public virtual ICollection<PromptVersionComment> Comments { get; set; }
    }
}
