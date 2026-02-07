using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("dictionary_versions")]
    public class DictionaryVersion
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("version")]
        public int Version { get; set; }

        [Required]
        [Column("changeType")]
        [StringLength(20)]
        public string ChangeType { get; set; }

        [Column("wordId")]
        public int? WordId { get; set; }

        [Column("changeData")]
        public string ChangeData { get; set; }

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; }
    }
}
