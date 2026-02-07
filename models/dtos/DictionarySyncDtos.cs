using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace serveur.Models.Dtos
{
    /// <summary>
    /// Information sur la version actuelle du dictionnaire
    /// </summary>
    public class DictionaryVersionDto
    {
        public int Version { get; set; }
        public int TotalWords { get; set; }
        public string Checksum { get; set; }
        public DateTime LastModified { get; set; }
    }

    /// <summary>
    /// Mot optimisé pour le transfert (noms courts pour réduire la taille JSON)
    /// </summary>
    public class DictionaryWordDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("o")]
        public string O { get; set; }  // Orthographe

        [JsonPropertyName("c")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string C { get; set; }  // Categorie

        [JsonPropertyName("fCp")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? FCp { get; set; }  // FrequenceCp

        [JsonPropertyName("fCe1")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? FCe1 { get; set; }  // FrequenceCe1

        [JsonPropertyName("fCe2")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? FCe2 { get; set; }  // FrequenceCe2Cm2

        [JsonPropertyName("fG")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? FG { get; set; }  // FrequenceGlobale

        [JsonPropertyName("p")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string P { get; set; }  // Phon

        [JsonPropertyName("ps")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Ps { get; set; }  // PhonSimplifiee
    }

    /// <summary>
    /// Export complet du dictionnaire
    /// </summary>
    public class DictionaryFullExportDto
    {
        public int Version { get; set; }
        public string Checksum { get; set; }
        public int TotalWords { get; set; }
        public List<DictionaryWordDto> Words { get; set; }
    }

    /// <summary>
    /// Changements depuis une version donnée
    /// </summary>
    public class DictionaryDeltaDto
    {
        public int FromVersion { get; set; }
        public int ToVersion { get; set; }
        public int ChangeCount { get; set; }
        public bool RequiresFullSync { get; set; }
        public string RequiresFullSyncReason { get; set; }
        public List<DictionaryChangeDto> Changes { get; set; }
    }

    /// <summary>
    /// Un changement individuel dans le dictionnaire
    /// </summary>
    public class DictionaryChangeDto
    {
        [JsonPropertyName("t")]
        public string Type { get; set; }  // ADD, UPDATE, DELETE

        [JsonPropertyName("id")]
        public int WordId { get; set; }

        [JsonPropertyName("w")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DictionaryWordDto Word { get; set; }  // null pour DELETE
    }

    /// <summary>
    /// Notification SignalR pour mise à jour du dictionnaire
    /// </summary>
    public class DictionaryUpdatedNotification
    {
        public int Version { get; set; }
        public int ChangeCount { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
