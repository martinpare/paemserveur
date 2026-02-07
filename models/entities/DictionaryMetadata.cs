using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("dictionary_metadata")]
    public class DictionaryMetadata
    {
        [Key]
        [Column("id")]
        public int Id { get; set; } = 1;

        [Column("currentVersion")]
        public int CurrentVersion { get; set; }

        [Column("totalWords")]
        public int TotalWords { get; set; }

        [Column("checksum")]
        [StringLength(64)]
        public string Checksum { get; set; }

        [Column("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}
