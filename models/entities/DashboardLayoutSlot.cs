using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("dashboardLayoutSlots")]
    public class DashboardLayoutSlot
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("layoutId")]
        public int LayoutId { get; set; }

        [Required]
        [Column("componentName")]
        [StringLength(100)]
        public string ComponentName { get; set; }

        [Column("sortOrder")]
        public int SortOrder { get; set; }

        // Responsive columns (Quasar 12-column grid)
        [Column("colXs")]
        public int ColXs { get; set; } = 12;

        [Column("colSm")]
        public int? ColSm { get; set; }

        [Column("colMd")]
        public int ColMd { get; set; } = 6;

        [Column("colLg")]
        public int? ColLg { get; set; }

        [Column("colXl")]
        public int? ColXl { get; set; }

        // Offsets
        [Column("offsetXs")]
        public int OffsetXs { get; set; } = 0;

        [Column("offsetMd")]
        public int? OffsetMd { get; set; }

        // Component-specific configuration (JSON)
        [Column("componentConfig")]
        public string ComponentConfig { get; set; }

        // Navigation property
        [ForeignKey("LayoutId")]
        public virtual DashboardLayout Layout { get; set; }
    }
}
