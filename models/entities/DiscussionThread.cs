using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("discussionThreads")]
    public class DiscussionThread
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// Type d'entité à laquelle ce thread est rattaché (ex: "ValidationInstance", "PageContent", "Learner", etc.)
        /// </summary>
        [Required]
        [StringLength(100)]
        [Column("entityType")]
        public string EntityType { get; set; }

        /// <summary>
        /// ID de l'entité à laquelle ce thread est rattaché
        /// </summary>
        [Required]
        [Column("entityId")]
        public int EntityId { get; set; }

        /// <summary>
        /// Type de contexte optionnel (ex: "ValidationInstanceStep", "PageSection", null si pas de contexte)
        /// </summary>
        [StringLength(100)]
        [Column("contextType")]
        public string ContextType { get; set; }

        /// <summary>
        /// ID du contexte optionnel
        /// </summary>
        [Column("contextId")]
        public int? ContextId { get; set; }

        /// <summary>
        /// Titre optionnel du fil de discussion
        /// </summary>
        [StringLength(255)]
        [Column("title")]
        public string Title { get; set; }

        [Required]
        [Column("createdByUserId")]
        public int CreatedByUserId { get; set; }

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; }

        [Column("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Indique si le thread est fermé (plus de réponses possibles)
        /// </summary>
        [Required]
        [Column("isClosed")]
        public bool IsClosed { get; set; }

        // Navigation properties
        [ForeignKey("CreatedByUserId")]
        public virtual User CreatedByUser { get; set; }

        public virtual ICollection<DiscussionMessage> Messages { get; set; }
    }
}
