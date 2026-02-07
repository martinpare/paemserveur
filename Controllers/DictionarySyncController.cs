using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using serveur.Models.Dtos;
using serveur.Services;

namespace serveur.Controllers
{
    [ApiController]
    [Route("api/dictionary-sync")]
    [Authorize]
    public class DictionarySyncController : ControllerBase
    {
        private readonly IDictionarySyncService _syncService;
        private readonly ILogger<DictionarySyncController> _logger;

        public DictionarySyncController(
            IDictionarySyncService syncService,
            ILogger<DictionarySyncController> logger)
        {
            _syncService = syncService;
            _logger = logger;
        }

        /// <summary>
        /// Obtient la version actuelle du dictionnaire avec métadonnées
        /// </summary>
        /// <returns>Version, nombre de mots, checksum et date de dernière modification</returns>
        [HttpGet("version")]
        [ResponseCache(Duration = 60)]
        public async Task<ActionResult<DictionaryVersionDto>> GetVersion()
        {
            var version = await _syncService.GetCurrentVersionAsync();
            return Ok(version);
        }

        /// <summary>
        /// Télécharge le dictionnaire complet
        /// Utilise la compression GZIP si supportée par le client
        /// </summary>
        /// <returns>Liste complète des mots avec leurs propriétés optimisées</returns>
        [HttpGet("full")]
        public async Task<ActionResult<DictionaryFullExportDto>> GetFullDictionary()
        {
            _logger.LogInformation("Demande de synchronisation complète du dictionnaire");

            var export = await _syncService.GetFullDictionaryAsync();

            // Ajouter header avec la version pour le client
            Response.Headers.Add("X-Dictionary-Version", export.Version.ToString());

            _logger.LogInformation(
                "Export complet du dictionnaire: {WordCount} mots, version {Version}",
                export.TotalWords, export.Version);

            return Ok(export);
        }

        /// <summary>
        /// Télécharge le dictionnaire en streaming (pour les très gros volumes)
        /// </summary>
        /// <returns>Stream de mots</returns>
        [HttpGet("stream")]
        public async IAsyncEnumerable<DictionaryWordDto> StreamDictionary()
        {
            _logger.LogInformation("Demande de streaming du dictionnaire");

            var version = await _syncService.GetCurrentVersionAsync();
            Response.Headers.Add("X-Dictionary-Version", version.Version.ToString());

            await foreach (var word in _syncService.StreamFullDictionaryAsync())
            {
                yield return word;
            }
        }

        /// <summary>
        /// Obtient les changements depuis une version donnée (delta sync)
        /// </summary>
        /// <param name="fromVersion">Version à partir de laquelle récupérer les changements</param>
        /// <returns>Liste des changements (ADD, UPDATE, DELETE) avec les données</returns>
        [HttpGet("delta/{fromVersion:int}")]
        public async Task<ActionResult<DictionaryDeltaDto>> GetDelta(int fromVersion)
        {
            if (fromVersion < 0)
            {
                return BadRequest("La version doit être positive");
            }

            _logger.LogInformation(
                "Demande de delta depuis la version {FromVersion}",
                fromVersion);

            var delta = await _syncService.GetDeltaAsync(fromVersion);

            if (delta.RequiresFullSync)
            {
                _logger.LogInformation(
                    "Delta trop important ou trop ancien, full sync requis: {Reason}",
                    delta.RequiresFullSyncReason);
            }
            else
            {
                _logger.LogInformation(
                    "Delta généré: {ChangeCount} changements de v{From} à v{To}",
                    delta.ChangeCount, fromVersion, delta.ToVersion);
            }

            return Ok(delta);
        }

        /// <summary>
        /// Recalcule et met à jour le checksum du dictionnaire
        /// (utile après un import manuel ou une modification en masse)
        /// </summary>
        [HttpPost("recompute-checksum")]
        public async Task<ActionResult<string>> RecomputeChecksum()
        {
            _logger.LogInformation("Recalcul du checksum du dictionnaire demandé");

            var checksum = await _syncService.ComputeChecksumAsync();

            _logger.LogInformation("Nouveau checksum: {Checksum}", checksum);

            return Ok(new { checksum });
        }
    }
}
