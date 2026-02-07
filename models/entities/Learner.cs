using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("learners")]
    public class Learner
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("permanentCode")]
        [StringLength(20)]
        public string PermanentCode { get; set; }

        [Required]
        [Column("firstName")]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [Column("lastName")]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        [Column("dateOfBirth")]
        public DateTime DateOfBirth { get; set; }

        [Column("email")]
        [StringLength(255)]
        public string Email { get; set; }

        [Column("nativeUserCode")]
        [StringLength(50)]
        public string NativeUserCode { get; set; }

        [Column("nativePassword")]
        [StringLength(255)]
        public string NativePassword { get; set; }

        [Required]
        [Column("learningCenterId")]
        public int LearningCenterId { get; set; }

        [Column("groupId")]
        public int? GroupId { get; set; }

        [Required]
        [Column("languageId")]
        public int LanguageId { get; set; }

        [Column("hasAccommodations")]
        public bool HasAccommodations { get; set; } = false;

        [Column("isManualEntry")]
        public bool IsManualEntry { get; set; } = false;

        [Column("isActive")]
        public bool IsActive { get; set; } = true;

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("modifiedAt")]
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

        [Column("avatar")]
        public string Avatar { get; set; }

        [Required]
        [Column("genderId")]
        public int GenderId { get; set; }

        // Navigation properties
        [ForeignKey("LearningCenterId")]
        public virtual LearningCenter LearningCenter { get; set; }

        [ForeignKey("GroupId")]
        public virtual Group Group { get; set; }

        [ForeignKey("LanguageId")]
        public virtual ValueDomainItem Language { get; set; }

        [ForeignKey("GenderId")]
        public virtual ValueDomainItem Gender { get; set; }
    }
}
