using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("pedagogicalStructures")]
    public class PedagogicalStructure
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("nameFr")]
        [StringLength(255)]
        public string NameFr { get; set; }

        [Required]
        [Column("nameEn")]
        [StringLength(255)]
        public string NameEn { get; set; }

        [Required]
        [Column("pedagogicalElementTypeId")]
        public int PedagogicalElementTypeId { get; set; }

        [Column("sectorCode")]
        [StringLength(10)]
        public string SectorCode { get; set; }

        [Column("parentId")]
        public int? ParentId { get; set; }

        [Column("organisationId")]
        public int? OrganisationId { get; set; }

        [Column("sortOrder")]
        public int? SortOrder { get; set; }

        [Column("createdAt")]
        public DateTime? CreatedAt { get; set; }

        // Navigation properties
        [ForeignKey("PedagogicalElementTypeId")]
        public virtual PedagogicalElementType PedagogicalElementType { get; set; }

        [ForeignKey("ParentId")]
        public virtual PedagogicalStructure Parent { get; set; }

        [ForeignKey("OrganisationId")]
        public virtual Organisation Organisation { get; set; }
    }
}
