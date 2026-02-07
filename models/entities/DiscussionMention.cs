using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("discussionMentions")]
    public class DiscussionMention
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("messageId")]
        public int MessageId { get; set; }

        [Required]
        [Column("mentionedUserId")]
        public int MentionedUserId { get; set; }

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Indique si la mention a été lue par l'utilisateur mentionné
        /// </summary>
        [Required]
        [Column("isRead")]
        public bool IsRead { get; set; }

        [Column("readAt")]
        public DateTime? ReadAt { get; set; }

        // Navigation properties
        [ForeignKey("MessageId")]
        public virtual DiscussionMessage Message { get; set; }

        [ForeignKey("MentionedUserId")]
        public virtual User MentionedUser { get; set; }
    }
}
