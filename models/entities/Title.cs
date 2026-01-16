using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("titles")]
    public class Title
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("parentId")]
        public int? ParentId { get; set; }

        [Required]
        [Column("code")]
        [StringLength(50)]
        public string Code { get; set; }

        [Required]
        [Column("maleLabelFr")]
        [StringLength(100)]
        public string MaleLabelFr { get; set; }

        [Required]
        [Column("femaleLabelFr")]
        [StringLength(100)]
        public string FemaleLabelFr { get; set; }

        [Required]
        [Column("maleLabelEn")]
        [StringLength(100)]
        public string MaleLabelEn { get; set; }

        [Required]
        [Column("femaleLabelEn")]
        [StringLength(100)]
        public string FemaleLabelEn { get; set; }

        [Required]
        [Column("order")]
        public int Order { get; set; }

        [Required]
        [Column("isActive")]
        public bool IsActive { get; set; }

        // Navigation properties
        [ForeignKey("ParentId")]
        public virtual Title Parent { get; set; }
    }
}
