using System;
using System.ComponentModel.DataAnnotations;

namespace serveur.Models.Dtos
{
    /// <summary>
    /// DTO pour afficher une annotation de texte
    /// </summary>
    public class TextAnnotationDto
    {
        public int Id { get; set; }
        public int LearnerId { get; set; }
        public string ContextId { get; set; }
        public string AnnotationType { get; set; }
        public string Color { get; set; }
        public string TextContent { get; set; }
        public string XPath { get; set; }
        public int StartOffset { get; set; }
        public int EndOffset { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO pour créer une annotation de texte
    /// </summary>
    public class TextAnnotationCreateDto
    {
        [Required]
        public int LearnerId { get; set; }

        [Required]
        [StringLength(255)]
        public string ContextId { get; set; }

        [Required]
        [StringLength(50)]
        public string AnnotationType { get; set; }

        [Required]
        [StringLength(20)]
        public string Color { get; set; }

        [Required]
        public string TextContent { get; set; }

        [Required]
        [StringLength(1000)]
        public string XPath { get; set; }

        [Required]
        public int StartOffset { get; set; }

        [Required]
        public int EndOffset { get; set; }
    }

    /// <summary>
    /// DTO pour mettre à jour une annotation de texte (couleur et type seulement)
    /// </summary>
    public class TextAnnotationUpdateDto
    {
        [Required]
        [StringLength(50)]
        public string AnnotationType { get; set; }

        [Required]
        [StringLength(20)]
        public string Color { get; set; }
    }
}
