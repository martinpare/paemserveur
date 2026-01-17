using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("firstName")]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [Column("lastName")]
        [StringLength(100)]
        public string LastName { get; set; }

        [Column("sexe")]
        [StringLength(1)]
        public string Sexe { get; set; }

        [Required]
        [Column("username")]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [Column("mail")]
        [StringLength(255)]
        public string Mail { get; set; }

        [Required]
        [Column("password")]
        [StringLength(255)]
        public string Password { get; set; }

        [Required]
        [Column("darkMode")]
        public bool DarkMode { get; set; }

        [Column("avatar")]
        public string Avatar { get; set; }

        [Column("accentColor")]
        [StringLength(20)]
        public string AccentColor { get; set; }

        [Column("language")]
        [StringLength(10)]
        public string Language { get; set; }

        [Column("organisationId")]
        public int? OrganisationId { get; set; }

        [Column("pedagogicalStructureId")]
        public int? PedagogicalStructureId { get; set; }

        [Column("learningCenterId")]
        public int? LearningCenterId { get; set; }

        [Column("titleId")]
        public int? TitleId { get; set; }

        [Column("activeRoleId")]
        public int? ActiveRoleId { get; set; }

        [Column("resetToken")]
        [StringLength(255)]
        public string ResetToken { get; set; }

        [Column("resetTokenExpiry")]
        public DateTime? ResetTokenExpiry { get; set; }

        [Required]
        [Column("active")]
        public bool Active { get; set; } = true;

        // Navigation properties
        [ForeignKey("OrganisationId")]
        public virtual Organisation Organisation { get; set; }

        [ForeignKey("PedagogicalStructureId")]
        public virtual PedagogicalStructure PedagogicalStructure { get; set; }

        [ForeignKey("LearningCenterId")]
        public virtual LearningCenter LearningCenter { get; set; }

        [ForeignKey("TitleId")]
        public virtual Title Title { get; set; }

        [ForeignKey("ActiveRoleId")]
        public virtual Role ActiveRole { get; set; }
    }
}
