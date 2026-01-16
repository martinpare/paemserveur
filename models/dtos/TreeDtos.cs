using System.Collections.Generic;

namespace serveur.Models.Dtos
{
    /// <summary>
    /// DTO pour afficher les rôles en structure arborescente
    /// </summary>
    public class RoleTreeDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string NameFr { get; set; }
        public string NameEn { get; set; }
        public string DescriptionFr { get; set; }
        public string DescriptionEn { get; set; }
        public int? OrganisationId { get; set; }
        public int Level { get; set; }
        public bool IsSystem { get; set; }
        public bool IsActive { get; set; }
        public bool HasAllPermissions { get; set; }
        public List<RoleTreeDto> Children { get; set; } = new List<RoleTreeDto>();
    }

    /// <summary>
    /// DTO pour afficher les fonctions en structure arborescente
    /// </summary>
    public class FunctionTreeDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string LabelFr { get; set; }
        public string LabelEn { get; set; }
        public string Icon { get; set; }
        public string Route { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public List<FunctionTreeDto> Children { get; set; } = new List<FunctionTreeDto>();
    }

    /// <summary>
    /// DTO pour afficher les titres en structure arborescente
    /// </summary>
    public class TitleTreeDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string MaleLabelFr { get; set; }
        public string FemaleLabelFr { get; set; }
        public string MaleLabelEn { get; set; }
        public string FemaleLabelEn { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public List<TitleTreeDto> Children { get; set; } = new List<TitleTreeDto>();
    }

    /// <summary>
    /// DTO pour afficher les structures pédagogiques en structure arborescente
    /// </summary>
    public class PedagogicalStructureTreeDto
    {
        public int Id { get; set; }
        public string NameFr { get; set; }
        public string NameEn { get; set; }
        public int PedagogicalElementTypeId { get; set; }
        public string SectorCode { get; set; }
        public int? SortOrder { get; set; }
        public int? OrganisationId { get; set; }
        public List<PedagogicalStructureTreeDto> Children { get; set; } = new List<PedagogicalStructureTreeDto>();
    }

    /// <summary>
    /// DTO pour afficher les noeuds de classification en structure arborescente
    /// </summary>
    public class ClassificationNodeTreeDto
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string NameFr { get; set; }
        public string NameEn { get; set; }
        public string DescriptionFr { get; set; }
        public string DescriptionEn { get; set; }
        public int SortOrder { get; set; }
        public string Weight { get; set; }
        public string ReferencesJuridiques { get; set; }
        public List<ClassificationNodeTreeDto> Children { get; set; } = new List<ClassificationNodeTreeDto>();
    }
}
