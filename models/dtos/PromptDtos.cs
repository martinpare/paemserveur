using System;
using System.Collections.Generic;

namespace serveur.Models.Dtos
{
    // Prompt DTOs
    public class PromptDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string NameFr { get; set; }
        public string NameEn { get; set; }
        public string DescriptionFr { get; set; }
        public string DescriptionEn { get; set; }
        public string Content { get; set; }
    }

    public class PromptCreateDto
    {
        public string Code { get; set; }
        public string NameFr { get; set; }
        public string NameEn { get; set; }
        public string DescriptionFr { get; set; }
        public string DescriptionEn { get; set; }
        public string Content { get; set; }
    }

    public class PromptUpdateDto
    {
        public string Code { get; set; }
        public string NameFr { get; set; }
        public string NameEn { get; set; }
        public string DescriptionFr { get; set; }
        public string DescriptionEn { get; set; }
        public string Content { get; set; }
    }

    public class PromptWithVersionsDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string NameFr { get; set; }
        public string NameEn { get; set; }
        public string DescriptionFr { get; set; }
        public string DescriptionEn { get; set; }
        public string Content { get; set; }
        public List<PromptVersionDto> Versions { get; set; }
    }

    // PromptVersion DTOs
    public class PromptVersionDto
    {
        public int Id { get; set; }
        public int PromptId { get; set; }
        public string Version { get; set; }
        public string NewContent { get; set; }
        public bool? Active { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class PromptVersionCreateDto
    {
        public int PromptId { get; set; }
        public string Version { get; set; }
        public string NewContent { get; set; }
        public bool? Active { get; set; }
    }

    public class PromptVersionUpdateDto
    {
        public string Version { get; set; }
        public string NewContent { get; set; }
        public bool? Active { get; set; }
    }

    public class PromptVersionWithCommentsDto
    {
        public int Id { get; set; }
        public int PromptId { get; set; }
        public string Version { get; set; }
        public string NewContent { get; set; }
        public bool? Active { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<PromptVersionCommentDto> Comments { get; set; }
    }

    // PromptVersionComment DTOs
    public class PromptVersionCommentDto
    {
        public int Id { get; set; }
        public int PromptVersionId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class PromptVersionCommentCreateDto
    {
        public int PromptVersionId { get; set; }
        public string Content { get; set; }
    }

    public class PromptVersionCommentUpdateDto
    {
        public string Content { get; set; }
    }
}
