using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace serveur.Models.Dtos
{
    /// <summary>
    /// DTO pour afficher un layout de dashboard
    /// </summary>
    public class DashboardLayoutDto
    {
        public int Id { get; set; }
        public int? RoleId { get; set; }
        public string PageKey { get; set; }
        public string NameFr { get; set; }
        public string NameEn { get; set; }
        public int SortOrder { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public List<DashboardLayoutSlotDto> Slots { get; set; }
    }

    /// <summary>
    /// DTO pour créer un layout
    /// </summary>
    public class DashboardLayoutCreateDto
    {
        public int? RoleId { get; set; }

        [Required]
        [StringLength(100)]
        public string PageKey { get; set; }

        [Required]
        [StringLength(100)]
        public string NameFr { get; set; }

        [Required]
        [StringLength(100)]
        public string NameEn { get; set; }

        public int SortOrder { get; set; } = 0;
        public bool IsDefault { get; set; } = false;
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// DTO pour mettre à jour un layout
    /// </summary>
    public class DashboardLayoutUpdateDto
    {
        public int? RoleId { get; set; }

        [Required]
        [StringLength(100)]
        public string PageKey { get; set; }

        [Required]
        [StringLength(100)]
        public string NameFr { get; set; }

        [Required]
        [StringLength(100)]
        public string NameEn { get; set; }

        public int SortOrder { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO pour afficher un slot de layout
    /// </summary>
    public class DashboardLayoutSlotDto
    {
        public int Id { get; set; }
        public int LayoutId { get; set; }
        public string ComponentName { get; set; }
        public int SortOrder { get; set; }
        public int ColXs { get; set; }
        public int? ColSm { get; set; }
        public int ColMd { get; set; }
        public int? ColLg { get; set; }
        public int? ColXl { get; set; }
        public int OffsetXs { get; set; }
        public int? OffsetMd { get; set; }
        public string ComponentConfig { get; set; }
    }

    /// <summary>
    /// DTO pour créer un slot
    /// </summary>
    public class DashboardLayoutSlotCreateDto
    {
        [Required]
        public int LayoutId { get; set; }

        [Required]
        [StringLength(100)]
        public string ComponentName { get; set; }

        public int SortOrder { get; set; } = 0;
        public int ColXs { get; set; } = 12;
        public int? ColSm { get; set; }
        public int ColMd { get; set; } = 6;
        public int? ColLg { get; set; }
        public int? ColXl { get; set; }
        public int OffsetXs { get; set; } = 0;
        public int? OffsetMd { get; set; }
        public string ComponentConfig { get; set; }
    }

    /// <summary>
    /// DTO pour mettre à jour un slot
    /// </summary>
    public class DashboardLayoutSlotUpdateDto
    {
        [Required]
        [StringLength(100)]
        public string ComponentName { get; set; }

        public int SortOrder { get; set; }
        public int ColXs { get; set; }
        public int? ColSm { get; set; }
        public int ColMd { get; set; }
        public int? ColLg { get; set; }
        public int? ColXl { get; set; }
        public int OffsetXs { get; set; }
        public int? OffsetMd { get; set; }
        public string ComponentConfig { get; set; }
    }

    /// <summary>
    /// DTO pour synchroniser les slots d'un layout
    /// </summary>
    public class SyncLayoutSlotsDto
    {
        [Required]
        public List<DashboardLayoutSlotCreateDto> Slots { get; set; }
    }
}
