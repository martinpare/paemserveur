using System.Collections.Generic;

namespace serveur.Models.Dtos
{
    public class CreateDocumentDto
    {
        public int? PedagogicalStructureId { get; set; }
        public int? DocumentTypeId { get; set; }
        public string ExternalReferenceCode { get; set; }
        public string Version { get; set; }
        public bool IsActive { get; set; }
        public int? Status { get; set; }
        public string TitleFr { get; set; }
        public string TitleEn { get; set; }
        public string DescriptionFr { get; set; }
        public string DescriptionEn { get; set; }
        public string WelcomeMessageFr { get; set; }
        public string WelcomeMessageEn { get; set; }
        public string CopyrightFr { get; set; }
        public string CopyrightEn { get; set; }
        public string UrlFr { get; set; }
        public string UrlEn { get; set; }
        public bool IsDownloadable { get; set; }
        public bool IsPublic { get; set; }
        public bool IsEditable { get; set; }
        public string EditorSettings { get; set; }
        public int? AuthorId { get; set; }
        public bool IsTemplate { get; set; }
        public List<int> ItemBankIds { get; set; }
    }

    public class UpdateDocumentDto
    {
        public int? PedagogicalStructureId { get; set; }
        public int? DocumentTypeId { get; set; }
        public string ExternalReferenceCode { get; set; }
        public string Version { get; set; }
        public bool IsActive { get; set; }
        public int? Status { get; set; }
        public string TitleFr { get; set; }
        public string TitleEn { get; set; }
        public string DescriptionFr { get; set; }
        public string DescriptionEn { get; set; }
        public string WelcomeMessageFr { get; set; }
        public string WelcomeMessageEn { get; set; }
        public string CopyrightFr { get; set; }
        public string CopyrightEn { get; set; }
        public string UrlFr { get; set; }
        public string UrlEn { get; set; }
        public bool IsDownloadable { get; set; }
        public bool IsPublic { get; set; }
        public bool IsEditable { get; set; }
        public string EditorSettings { get; set; }
        public int? AuthorId { get; set; }
        public bool IsTemplate { get; set; }
        public List<int> ItemBankIds { get; set; }
    }

    public class DocumentResponseDto
    {
        public int Id { get; set; }
        public int? PedagogicalStructureId { get; set; }
        public int? DocumentTypeId { get; set; }
        public string ExternalReferenceCode { get; set; }
        public string Version { get; set; }
        public bool IsActive { get; set; }
        public int? Status { get; set; }
        public string TitleFr { get; set; }
        public string TitleEn { get; set; }
        public string DescriptionFr { get; set; }
        public string DescriptionEn { get; set; }
        public string WelcomeMessageFr { get; set; }
        public string WelcomeMessageEn { get; set; }
        public string CopyrightFr { get; set; }
        public string CopyrightEn { get; set; }
        public string UrlFr { get; set; }
        public string UrlEn { get; set; }
        public bool IsDownloadable { get; set; }
        public bool IsPublic { get; set; }
        public bool IsEditable { get; set; }
        public string EditorSettings { get; set; }
        public int? AuthorId { get; set; }
        public bool IsTemplate { get; set; }
        public System.DateTime? CreatedAt { get; set; }
        public System.DateTime? UpdatedAt { get; set; }
        public List<ItemBankSummaryDto> ItemBanks { get; set; }
    }

    public class ItemBankSummaryDto
    {
        public int Id { get; set; }
        public string NameFr { get; set; }
        public string NameEn { get; set; }
    }
}
