using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("documentMedias")]
    public class DocumentMedia
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("documentId")]
        public int DocumentId { get; set; }

        [Required]
        [Column("fileName")]
        [StringLength(255)]
        public string FileName { get; set; }

        [Column("mimeType")]
        [StringLength(100)]
        public string MimeType { get; set; }

        [Column("fileSize")]
        public int? FileSize { get; set; }

        [Column("dataUrl")]
        public string DataUrl { get; set; }

        [Column("width")]
        public int? Width { get; set; }

        [Column("height")]
        public int? Height { get; set; }

        [Column("uploadedAt")]
        public DateTime? UploadedAt { get; set; }

        // Navigation properties
        [ForeignKey("DocumentId")]
        public virtual Document Document { get; set; }
    }
}
