using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("examMessages")]
    public class ExamMessage
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("examSessionId")]
        public int ExamSessionId { get; set; }

        [Required]
        [Column("senderType")]
        [StringLength(20)]
        public string SenderType { get; set; }

        [Required]
        [Column("senderId")]
        public int SenderId { get; set; }

        [Column("recipientType")]
        [StringLength(20)]
        public string RecipientType { get; set; }

        [Column("recipientId")]
        public int? RecipientId { get; set; }

        [Required]
        [Column("messageText")]
        public string MessageText { get; set; }

        [Column("isRead")]
        public bool IsRead { get; set; }

        [Required]
        [Column("createdAt")]
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        [ForeignKey("ExamSessionId")]
        public virtual ExamSession ExamSession { get; set; }
    }
}
