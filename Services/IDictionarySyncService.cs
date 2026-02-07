using System.Collections.Generic;
using System.Threading.Tasks;
using serveur.Models.Dtos;

namespace serveur.Services
{
    public interface IDictionarySyncService
    {
        /// <summary>
        /// Obtient la version actuelle du dictionnaire avec métadonnées
        /// </summary>
        Task<DictionaryVersionDto> GetCurrentVersionAsync();

        /// <summary>
        /// Obtient le dictionnaire complet sous forme de liste
        /// </summary>
        Task<DictionaryFullExportDto> GetFullDictionaryAsync();

        /// <summary>
        /// Stream le dictionnaire complet pour éviter de charger tout en mémoire
        /// </summary>
        IAsyncEnumerable<DictionaryWordDto> StreamFullDictionaryAsync();

        /// <summary>
        /// Obtient les changements depuis une version donnée
        /// </summary>
        /// <param name="fromVersion">Version de départ</param>
        Task<DictionaryDeltaDto> GetDeltaAsync(int fromVersion);

        /// <summary>
        /// Notifie les clients connectés d'une mise à jour du dictionnaire
        /// </summary>
        /// <param name="newVersion">Nouvelle version</param>
        /// <param name="changeCount">Nombre de changements</param>
        Task NotifyDictionaryChangedAsync(int newVersion, int changeCount);

        /// <summary>
        /// Calcule le checksum du dictionnaire actuel
        /// </summary>
        Task<string> ComputeChecksumAsync();
    }
}
