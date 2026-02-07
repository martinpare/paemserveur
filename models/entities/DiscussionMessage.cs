using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("discussionMessages")]
    public class DiscussionMessage
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("threadId")]
        public int ThreadId { get; set; }

        /// <summary>
        /// ID du message parent (pour les réponses imbriquées)
        /// </summary>
        [Column("parentMessageId")]
        public int? ParentMessageId { get; set; }

        [Required]
        [Column("authorUserId")]
        public int AuthorUserId { get; set; }

        [Required]
        [Column("content")]
        public string Content { get; set; }

        [Required]
        [Column("isEdited")]
        public bool IsEdited { get; set; }

        [Column("editedAt")]
        public DateTime? EditedAt { get; set; }

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; }

        [Column("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Indique si le message a été supprimé (soft delete)
        /// </summary>
        [Required]
        [Column("isDeleted")]
        public bool IsDeleted { get; set; }

        [Column("deletedAt")]
        public DateTime? DeletedAt { get; set; }

        // Navigation properties
        [ForeignKey("ThreadId")]
        public virtual DiscussionThread Thread { get; set; }

        [ForeignKey("ParentMessageId")]
        public virtual DiscussionMessage ParentMessage { get; set; }

        [ForeignKey("AuthorUserId")]
        public virtual User AuthorUser { get; set; }

        public virtual ICollection<DiscussionMessage> Replies { get; set; }
        public virtual ICollection<DiscussionMention> Mentions { get; set; }
    }
}
