using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("dashboardLayouts")]
    public class DashboardLayout
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("roleId")]
        public int? RoleId { get; set; }

        [Required]
        [Column("pageKey")]
        [StringLength(100)]
        public string PageKey { get; set; }

        [Required]
        [Column("nameFr")]
        [StringLength(100)]
        public string NameFr { get; set; }

        [Required]
        [Column("nameEn")]
        [StringLength(100)]
        public string NameEn { get; set; }

        [Column("sortOrder")]
        public int SortOrder { get; set; }

        [Column("isDefault")]
        public bool IsDefault { get; set; }

        [Column("isActive")]
        public bool IsActive { get; set; }

        // Navigation properties
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }

        public virtual ICollection<DashboardLayoutSlot> Slots { get; set; }
    }
}
