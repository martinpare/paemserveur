using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("prompts")]
    public class Prompt
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("code")]
        [StringLength(255)]
        public string Code { get; set; }

        [Required]
        [Column("nameFr")]
        [StringLength(255)]
        public string NameFr { get; set; }

        [Required]
        [Column("nameEn")]
        [StringLength(255)]
        public string NameEn { get; set; }

        [Column("descriptionFr")]
        public string DescriptionFr { get; set; }

        [Column("descriptionEn")]
        public string DescriptionEn { get; set; }

        [Column("content")]
        public string Content { get; set; }

        // Navigation properties
        public virtual ICollection<PromptVersion> Versions { get; set; }
    }
}
