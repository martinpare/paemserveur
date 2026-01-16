using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("functions")]
    public class Function
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("code")]
        [StringLength(50)]
        public string Code { get; set; }

        [Required]
        [Column("labelFr")]
        [StringLength(100)]
        public string LabelFr { get; set; }

        [Required]
        [Column("labelEn")]
        [StringLength(100)]
        public string LabelEn { get; set; }

        [Column("descriptionFr")]
        [StringLength(500)]
        public string DescriptionFr { get; set; }

        [Column("descriptionEn")]
        [StringLength(500)]
        public string DescriptionEn { get; set; }

        [Column("parentId")]
        public int? ParentId { get; set; }

        [Required]
        [Column("sortOrder")]
        public int SortOrder { get; set; }

        [Column("icon")]
        [StringLength(50)]
        public string Icon { get; set; }

        [Column("route")]
        [StringLength(255)]
        public string Route { get; set; }

        [Required]
        [Column("isActive")]
        public bool IsActive { get; set; }

        // Navigation properties
        [ForeignKey("ParentId")]
        public virtual Function Parent { get; set; }
    }
}
