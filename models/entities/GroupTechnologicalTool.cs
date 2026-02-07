using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("groupTechnologicalTools")]
    public class GroupTechnologicalTool
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("groupId")]
        public int GroupId { get; set; }

        [Required]
        [Column("technologicalToolId")]
        public int TechnologicalToolId { get; set; }

        [Column("createdAt")]
        public DateTime? CreatedAt { get; set; }

        // Navigation properties
        [ForeignKey("GroupId")]
        public virtual Group Group { get; set; }

        [ForeignKey("TechnologicalToolId")]
        public virtual TechnologicalTool TechnologicalTool { get; set; }
    }
}
