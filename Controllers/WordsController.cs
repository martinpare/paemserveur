using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using serveur.Data;
using serveur.Models.Dtos;
using serveur.Models.Entities;

namespace serveur.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WordsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<WordsController> _logger;

        public WordsController(AppDbContext context, ILogger<WordsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir des prédictions de mots basées sur un préfixe
        /// </summary>
        /// <param name="prefix">Le début du mot à rechercher</param>
        /// <param name="niveauScolaire">Niveau scolaire : CP, CE1, CE2CM2 ou Global (défaut)</param>
        /// <param name="maxResults">Nombre maximum de résultats (défaut 10)</param>
        [HttpGet("predictions")]
        public async Task<ActionResult<IEnumerable<WordPredictionDto>>> GetPredictions(
            [FromQuery] string prefix,
            [FromQuery] string niveauScolaire = "Global",
            [FromQuery] int maxResults = 10)
        {
            if (string.IsNullOrWhiteSpace(prefix))
            {
                return BadRequest("Le préfixe est requis");
            }

            if (maxResults < 1 || maxResults > 100)
            {
                maxResults = 10;
            }

            try
            {
                var predictions = new List<WordPredictionDto>();

                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "EXEC dbo.getWordPredictions @prefix, @niveauScolaire, @maxResults";
                    command.CommandType = System.Data.CommandType.Text;

                    command.Parameters.Add(new SqlParameter("@prefix", prefix));
                    command.Parameters.Add(new SqlParameter("@niveauScolaire", niveauScolaire));
                    command.Parameters.Add(new SqlParameter("@maxResults", maxResults));

                    await _context.Database.OpenConnectionAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            predictions.Add(new WordPredictionDto
                            {
                                Orthographe = reader.GetString(0),
                                Categorie = reader.IsDBNull(1) ? null : reader.GetString(1),
                                Frequence = reader.GetDecimal(2),
                                Phon = reader.IsDBNull(3) ? null : reader.GetString(3),
                                PhonSimplifiee = reader.IsDBNull(4) ? null : reader.GetString(4)
                            });
                        }
                    }
                }

                return Ok(predictions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des prédictions pour le préfixe {Prefix}", prefix);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Importer des mots en masse (format Manulex)
        /// </summary>
        /// <param name="importDto">Liste des mots à importer</param>
        [HttpPost("import")]
        public async Task<ActionResult<WordImportResultDto>> ImportWords([FromBody] WordBulkImportDto importDto)
        {
            if (importDto?.Words == null || !importDto.Words.Any())
            {
                return BadRequest("Aucun mot à importer");
            }

            var result = new WordImportResultDto
            {
                TotalProcessed = importDto.Words.Count,
                Inserted = 0,
                Updated = 0,
                Errors = 0,
                ErrorMessages = new List<string>()
            };

            try
            {
                foreach (var wordDto in importDto.Words)
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(wordDto.Orthographe))
                        {
                            result.Errors++;
                            result.ErrorMessages.Add($"Mot vide ignoré");
                            continue;
                        }

                        var existingWord = await _context.Words
                            .FirstOrDefaultAsync(w => w.Orthographe == wordDto.Orthographe && w.Categorie == wordDto.Categorie);

                        if (existingWord != null)
                        {
                            existingWord.FrequenceCp = wordDto.FrequenceCp;
                            existingWord.FrequenceCe1 = wordDto.FrequenceCe1;
                            existingWord.FrequenceCe2Cm2 = wordDto.FrequenceCe2Cm2;
                            existingWord.FrequenceGlobale = wordDto.FrequenceGlobale;
                            existingWord.Phon = wordDto.Phon;
                            existingWord.PhonSimplifiee = wordDto.PhonSimplifiee;
                            result.Updated++;
                        }
                        else
                        {
                            var newWord = new Word
                            {
                                Orthographe = wordDto.Orthographe,
                                Categorie = wordDto.Categorie,
                                FrequenceCp = wordDto.FrequenceCp,
                                FrequenceCe1 = wordDto.FrequenceCe1,
                                FrequenceCe2Cm2 = wordDto.FrequenceCe2Cm2,
                                FrequenceGlobale = wordDto.FrequenceGlobale,
                                Phon = wordDto.Phon,
                                PhonSimplifiee = wordDto.PhonSimplifiee
                            };
                            _context.Words.Add(newWord);
                            result.Inserted++;
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Errors++;
                        result.ErrorMessages.Add($"Erreur pour '{wordDto.Orthographe}': {ex.Message}");
                    }
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Import terminé: {Inserted} insérés, {Updated} mis à jour, {Errors} erreurs",
                    result.Inserted, result.Updated, result.Errors);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'import des mots");
                return StatusCode(500, $"Erreur lors de l'import: {ex.Message}");
            }
        }

        /// <summary>
        /// Importer un fichier eManulex (TSV/CSV avec fréquences par niveau scolaire)
        /// Format attendu: GRAM, ORTHO, CP-U, CE1-U, CE2 au CM2-U, CP au CM2-U
        /// ATTENTION: Cette opération remplace toutes les données existantes
        /// </summary>
        [HttpPost("import-manulex")]
        public async Task<ActionResult<WordImportResultDto>> ImportManulexFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Aucun fichier fourni");
            }

            var result = new WordImportResultDto
            {
                TotalProcessed = 0,
                Inserted = 0,
                Updated = 0,
                Errors = 0,
                ErrorMessages = new List<string>()
            };

            try
            {
                // Détecter l'encodage (UTF-16 pour les exports Excel, UTF-8 sinon)
                Encoding encoding;
                using (var detectStream = file.OpenReadStream())
                {
                    var bom = new byte[2];
                    detectStream.Read(bom, 0, 2);
                    encoding = (bom[0] == 0xFF && bom[1] == 0xFE) ? Encoding.Unicode : Encoding.UTF8;
                }

                // Parser tout le fichier en mémoire
                var wordsToInsert = new List<Word>();

                using (var stream = file.OpenReadStream())
                using (var reader = new StreamReader(stream, encoding))
                {
                    // Lire l'en-tête
                    var headerLine = await reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(headerLine))
                    {
                        return BadRequest("Fichier vide");
                    }

                    // Déterminer le séparateur (tab ou virgule)
                    char separator = headerLine.Contains('\t') ? '\t' : ',';

                    // Parser les lignes
                    string line;
                    int lineNumber = 1;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        lineNumber++;
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        try
                        {
                            var cols = line.Split(separator);
                            if (cols.Length < 2) continue;

                            var categorie = cols[0].Trim();
                            var orthographe = cols[1].Trim();

                            if (string.IsNullOrWhiteSpace(orthographe))
                            {
                                result.Errors++;
                                continue;
                            }

                            wordsToInsert.Add(new Word
                            {
                                Orthographe = orthographe,
                                Categorie = categorie,
                                FrequenceCp = ParseDecimal(cols.Length > 2 ? cols[2] : null),
                                FrequenceCe1 = ParseDecimal(cols.Length > 3 ? cols[3] : null),
                                FrequenceCe2Cm2 = ParseDecimal(cols.Length > 4 ? cols[4] : null),
                                FrequenceGlobale = ParseDecimal(cols.Length > 5 ? cols[5] : null)
                            });
                        }
                        catch (Exception ex)
                        {
                            result.Errors++;
                            if (result.ErrorMessages.Count < 10)
                            {
                                result.ErrorMessages.Add($"Ligne {lineNumber}: {ex.Message}");
                            }
                        }
                    }
                }

                _logger.LogInformation("Fichier parsé: {Count} mots à importer", wordsToInsert.Count);

                // Vider la table et insérer en bulk
                await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE dbo.words");
                _logger.LogInformation("Table words vidée");

                // Insérer par lots de 1000
                const int batchSize = 1000;
                for (int i = 0; i < wordsToInsert.Count; i += batchSize)
                {
                    var batch = wordsToInsert.Skip(i).Take(batchSize);
                    await _context.Words.AddRangeAsync(batch);
                    await _context.SaveChangesAsync();
                    _context.ChangeTracker.Clear();

                    _logger.LogInformation("Import eManulex: {Count}/{Total} mots insérés...",
                        Math.Min(i + batchSize, wordsToInsert.Count), wordsToInsert.Count);
                }

                result.TotalProcessed = wordsToInsert.Count;
                result.Inserted = wordsToInsert.Count;

                _logger.LogInformation("Import eManulex terminé: {Inserted} mots insérés", result.Inserted);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'import eManulex");
                return StatusCode(500, $"Erreur lors de l'import: {ex.Message}");
            }
        }

        /// <summary>
        /// Enrichir les mots avec la phonétique de Lexique383
        /// Format attendu: ortho, phon, ... (colonnes 0 et 1)
        /// </summary>
        [HttpPost("import-lexique-phonetique")]
        public async Task<ActionResult<WordImportResultDto>> ImportLexiquePhonetics(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Aucun fichier fourni");
            }

            var result = new WordImportResultDto
            {
                TotalProcessed = 0,
                Inserted = 0,
                Updated = 0,
                Errors = 0,
                ErrorMessages = new List<string>()
            };

            try
            {
                // Charger le dictionnaire phonétique en mémoire
                var phonDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                using (var stream = file.OpenReadStream())
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    // Lire l'en-tête
                    var headerLine = await reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(headerLine))
                    {
                        return BadRequest("Fichier vide");
                    }

                    var headers = headerLine.Split('\t');
                    int orthoIndex = Array.IndexOf(headers, "ortho");
                    int phonIndex = Array.IndexOf(headers, "phon");

                    if (orthoIndex < 0 || phonIndex < 0)
                    {
                        return BadRequest("Colonnes 'ortho' et 'phon' requises dans le fichier Lexique");
                    }

                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        var cols = line.Split('\t');
                        if (cols.Length <= Math.Max(orthoIndex, phonIndex)) continue;

                        var ortho = cols[orthoIndex].Trim().ToLowerInvariant();
                        var phon = cols[phonIndex].Trim();

                        if (!string.IsNullOrEmpty(ortho) && !string.IsNullOrEmpty(phon))
                        {
                            phonDict.TryAdd(ortho, phon);
                        }
                    }
                }

                _logger.LogInformation("Lexique383 chargé: {Count} entrées phonétiques", phonDict.Count);

                // Mettre à jour les mots en base par lots
                const int batchSize = 500;
                var words = await _context.Words.ToListAsync();

                foreach (var word in words)
                {
                    var orthoLower = word.Orthographe?.ToLowerInvariant();
                    if (orthoLower != null && phonDict.TryGetValue(orthoLower, out var phon))
                    {
                        word.Phon = phon;
                        word.PhonSimplifiee = SimplifyPhonetic(phon);
                        result.Updated++;
                    }
                    else
                    {
                        result.Errors++;
                    }
                    result.TotalProcessed++;

                    if (result.TotalProcessed % batchSize == 0)
                    {
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Enrichissement phonétique: {Count} mots traités...", result.TotalProcessed);
                    }
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Enrichissement phonétique terminé: {Updated} enrichis, {Errors} sans correspondance",
                    result.Updated, result.Errors);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'enrichissement phonétique");
                return StatusCode(500, $"Erreur lors de l'import: {ex.Message}");
            }
        }

        private static decimal? ParseDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            value = value.Trim().Replace(',', '.');
            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }
            return null;
        }

        private static string SimplifyPhonetic(string phon)
        {
            if (string.IsNullOrEmpty(phon)) return "";
            return phon
                .Replace("°", "")
                .Replace("'", "")
                .Replace("-", "")
                .Replace(" ", "")
                .ToUpperInvariant();
        }
    }
}
