using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace serveur.Models.Entities
{
    [Table("words")]
    public class Word
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("orthographe")]
        [StringLength(100)]
        public string Orthographe { get; set; }

        [Column("categorie")]
        [StringLength(20)]
        public string Categorie { get; set; }

        [Column("frequenceCp")]
        public decimal? FrequenceCp { get; set; }

        [Column("frequenceCe1")]
        public decimal? FrequenceCe1 { get; set; }

        [Column("frequenceCe2Cm2")]
        public decimal? FrequenceCe2Cm2 { get; set; }

        [Column("frequenceGlobale")]
        public decimal? FrequenceGlobale { get; set; }

        [Column("phon")]
        [StringLength(100)]
        public string Phon { get; set; }

        [Column("phonSimplifiee")]
        [StringLength(100)]
        public string PhonSimplifiee { get; set; }
    }
}
