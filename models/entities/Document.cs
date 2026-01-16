using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("documents")]
    public class Document
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("pedagogicalStrudtureId")]
        public int? PedagogicalStructureId { get; set; }

        [Column("documentTypeId")]
        public int? DocumentTypeId { get; set; }

        [Column("externalReferenceCode")]
        [StringLength(100)]
        public string ExternalReferenceCode { get; set; }

        [Column("version")]
        [StringLength(20)]
        public string Version { get; set; }

        [Required]
        [Column("isActive")]
        public bool IsActive { get; set; }

        [Column("status")]
        public int? Status { get; set; }

        [Required]
        [Column("titleFr")]
        [StringLength(255)]
        public string TitleFr { get; set; }

        [Required]
        [Column("titleEn")]
        [StringLength(255)]
        public string TitleEn { get; set; }

        [Column("descriptionFr")]
        public string DescriptionFr { get; set; }

        [Column("descriptionEn")]
        public string DescriptionEn { get; set; }

        [Column("welcomeMessageFr")]
        public string WelcomeMessageFr { get; set; }

        [Column("welcomeMessageEn")]
        public string WelcomeMessageEn { get; set; }

        [Column("copyrightFr")]
        public string CopyrightFr { get; set; }

        [Column("copyrightEn")]
        public string CopyrightEn { get; set; }

        [Column("urlFr")]
        public string UrlFr { get; set; }

        [Column("urlEn")]
        public string UrlEn { get; set; }

        [Required]
        [Column("isDownloadable")]
        public bool IsDownloadable { get; set; }

        [Required]
        [Column("isPublic")]
        public bool IsPublic { get; set; }

        [Column("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [Required]
        [Column("isEditable")]
        public bool IsEditable { get; set; }

        [Column("editorSettings")]
        public string EditorSettings { get; set; }

        [Column("authorId")]
        public int? AuthorId { get; set; }

        [Required]
        [Column("isTemplate")]
        public bool IsTemplate { get; set; }

        [Column("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("PedagogicalStructureId")]
        public virtual PedagogicalStructure PedagogicalStructure { get; set; }

        [ForeignKey("DocumentTypeId")]
        public virtual DocumentType DocumentType { get; set; }

        [ForeignKey("AuthorId")]
        public virtual User Author { get; set; }
    }
}
