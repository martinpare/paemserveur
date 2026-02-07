using System.Collections.Generic;

namespace serveur.Models.Dtos
{
    public class WordPredictionDto
    {
        public string Orthographe { get; set; }
        public string Categorie { get; set; }
        public decimal Frequence { get; set; }
        public string Phon { get; set; }
        public string PhonSimplifiee { get; set; }
    }

    public class WordImportDto
    {
        public string Orthographe { get; set; }
        public string Categorie { get; set; }
        public decimal? FrequenceCp { get; set; }
        public decimal? FrequenceCe1 { get; set; }
        public decimal? FrequenceCe2Cm2 { get; set; }
        public decimal? FrequenceGlobale { get; set; }
        public string Phon { get; set; }
        public string PhonSimplifiee { get; set; }
    }

    public class WordBulkImportDto
    {
        public List<WordImportDto> Words { get; set; }
    }

    public class WordImportResultDto
    {
        public int TotalProcessed { get; set; }
        public int Inserted { get; set; }
        public int Updated { get; set; }
        public int Errors { get; set; }
        public List<string> ErrorMessages { get; set; }
    }
}
