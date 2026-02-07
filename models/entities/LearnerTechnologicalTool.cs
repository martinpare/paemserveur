using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("learnerTechnologicalTools")]
    public class LearnerTechnologicalTool
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("learnerId")]
        public int LearnerId { get; set; }

        [Required]
        [Column("technologicalToolId")]
        public int TechnologicalToolId { get; set; }

        [Column("data")]
        public string Data { get; set; }

        [Column("createdAt")]
        public DateTime? CreatedAt { get; set; }

        // Navigation properties
        [ForeignKey("LearnerId")]
        public virtual Learner Learner { get; set; }

        [ForeignKey("TechnologicalToolId")]
        public virtual TechnologicalTool TechnologicalTool { get; set; }
    }
}
