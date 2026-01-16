using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("refresh_tokens")]
    public class RefreshToken
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("token")]
        [StringLength(500)]
        public string Token { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("expires_at")]
        public DateTime ExpiresAt { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("revoked_at")]
        public DateTime? RevokedAt { get; set; }

        [Column("is_revoked")]
        public bool IsRevoked { get; set; } = false;

        [Column("replaced_by_token")]
        [StringLength(500)]
        public string ReplacedByToken { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public bool IsActive => !IsRevoked && DateTime.UtcNow < ExpiresAt;
    }
}
