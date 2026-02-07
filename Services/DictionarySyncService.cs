using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using serveur.Data;
using serveur.Hubs;
using serveur.Models.Dtos;
using serveur.Models.Entities;

namespace serveur.Services
{
    public class DictionarySyncService : IDictionarySyncService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<DictionarySyncService> _logger;
        private const int DeltaRetentionDays = 30;
        private const int MaxDeltaChanges = 5000;

        public DictionarySyncService(
            AppDbContext context,
            IHubContext<NotificationHub> hubContext,
            ILogger<DictionarySyncService> logger)
        {
            _context = context;
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task<DictionaryVersionDto> GetCurrentVersionAsync()
        {
            var metadata = await _context.Set<DictionaryMetadata>()
                .FirstOrDefaultAsync();

            if (metadata == null)
            {
                // Initialiser si absent
                var totalWords = await _context.Words.CountAsync();
                metadata = new DictionaryMetadata
                {
                    Id = 1,
                    CurrentVersion = 1,
                    TotalWords = totalWords,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.Set<DictionaryMetadata>().Add(metadata);
                await _context.SaveChangesAsync();
            }

            return new DictionaryVersionDto
            {
                Version = metadata.CurrentVersion,
                TotalWords = metadata.TotalWords,
                Checksum = metadata.Checksum,
                LastModified = metadata.UpdatedAt
            };
        }

        public async Task<DictionaryFullExportDto> GetFullDictionaryAsync()
        {
            var metadata = await GetCurrentVersionAsync();

            var words = await _context.Words
                .AsNoTracking()
                .OrderBy(w => w.Orthographe)
                .Select(w => new DictionaryWordDto
                {
                    Id = w.Id,
                    O = w.Orthographe,
                    C = w.Categorie,
                    FCp = w.FrequenceCp,
                    FCe1 = w.FrequenceCe1,
                    FCe2 = w.FrequenceCe2Cm2,
                    FG = w.FrequenceGlobale,
                    P = w.Phon,
                    Ps = w.PhonSimplifiee
                })
                .ToListAsync();

            return new DictionaryFullExportDto
            {
                Version = metadata.Version,
                Checksum = metadata.Checksum,
                TotalWords = words.Count,
                Words = words
            };
        }

        public async IAsyncEnumerable<DictionaryWordDto> StreamFullDictionaryAsync()
        {
            const int batchSize = 5000;
            var totalWords = await _context.Words.CountAsync();
            var batches = (int)Math.Ceiling((double)totalWords / batchSize);

            for (int i = 0; i < batches; i++)
            {
                var words = await _context.Words
                    .AsNoTracking()
                    .OrderBy(w => w.Id)
                    .Skip(i * batchSize)
                    .Take(batchSize)
                    .Select(w => new DictionaryWordDto
                    {
                        Id = w.Id,
                        O = w.Orthographe,
                        C = w.Categorie,
                        FCp = w.FrequenceCp,
                        FCe1 = w.FrequenceCe1,
                        FCe2 = w.FrequenceCe2Cm2,
                        FG = w.FrequenceGlobale,
                        P = w.Phon,
                        Ps = w.PhonSimplifiee
                    })
                    .ToListAsync();

                foreach (var word in words)
                {
                    yield return word;
                }
            }
        }

        public async Task<DictionaryDeltaDto> GetDeltaAsync(int fromVersion)
        {
            var metadata = await GetCurrentVersionAsync();
            var currentVersion = metadata.Version;

            // Vérifier si le delta est trop ancien (> 30 jours)
            var cutoffDate = DateTime.UtcNow.AddDays(-DeltaRetentionDays);
            var oldestDelta = await _context.Set<DictionaryVersion>()
                .Where(v => v.Version > fromVersion)
                .OrderBy(v => v.CreatedAt)
                .FirstOrDefaultAsync();

            if (oldestDelta != null && oldestDelta.CreatedAt < cutoffDate)
            {
                return new DictionaryDeltaDto
                {
                    FromVersion = fromVersion,
                    ToVersion = currentVersion,
                    ChangeCount = 0,
                    RequiresFullSync = true,
                    RequiresFullSyncReason = "Les changements demandés sont trop anciens (> 30 jours). Une synchronisation complète est nécessaire.",
                    Changes = new List<DictionaryChangeDto>()
                };
            }

            // Récupérer les changements
            var changes = await _context.Set<DictionaryVersion>()
                .AsNoTracking()
                .Where(v => v.Version > fromVersion)
                .OrderBy(v => v.Version)
                .ThenBy(v => v.Id)
                .ToListAsync();

            // Si trop de changements, suggérer full sync
            if (changes.Count > MaxDeltaChanges)
            {
                return new DictionaryDeltaDto
                {
                    FromVersion = fromVersion,
                    ToVersion = currentVersion,
                    ChangeCount = changes.Count,
                    RequiresFullSync = true,
                    RequiresFullSyncReason = $"Trop de changements ({changes.Count}). Une synchronisation complète est recommandée.",
                    Changes = new List<DictionaryChangeDto>()
                };
            }

            // Convertir en DTOs
            var changeDtos = changes.Select(c => new DictionaryChangeDto
            {
                Type = c.ChangeType,
                WordId = c.WordId ?? 0,
                Word = c.ChangeType != "DELETE" && !string.IsNullOrEmpty(c.ChangeData)
                    ? ParseWordFromJson(c.ChangeData)
                    : null
            }).ToList();

            return new DictionaryDeltaDto
            {
                FromVersion = fromVersion,
                ToVersion = currentVersion,
                ChangeCount = changeDtos.Count,
                RequiresFullSync = false,
                Changes = changeDtos
            };
        }

        private DictionaryWordDto ParseWordFromJson(string json)
        {
            try
            {
                var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                return new DictionaryWordDto
                {
                    Id = root.TryGetProperty("id", out var id) ? id.GetInt32() : 0,
                    O = root.TryGetProperty("orthographe", out var ortho) ? ortho.GetString() : null,
                    C = root.TryGetProperty("categorie", out var cat) ? cat.GetString() : null,
                    FCp = root.TryGetProperty("frequenceCp", out var fcp) && fcp.ValueKind != JsonValueKind.Null ? fcp.GetDecimal() : null,
                    FCe1 = root.TryGetProperty("frequenceCe1", out var fce1) && fce1.ValueKind != JsonValueKind.Null ? fce1.GetDecimal() : null,
                    FCe2 = root.TryGetProperty("frequenceCe2Cm2", out var fce2) && fce2.ValueKind != JsonValueKind.Null ? fce2.GetDecimal() : null,
                    FG = root.TryGetProperty("frequenceGlobale", out var fg) && fg.ValueKind != JsonValueKind.Null ? fg.GetDecimal() : null,
                    P = root.TryGetProperty("phon", out var phon) ? phon.GetString() : null,
                    Ps = root.TryGetProperty("phonSimplifiee", out var phonS) ? phonS.GetString() : null
                };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Erreur lors du parsing du JSON du mot: {Json}", json);
                return null;
            }
        }

        public async Task NotifyDictionaryChangedAsync(int newVersion, int changeCount)
        {
            var notification = new DictionaryUpdatedNotification
            {
                Version = newVersion,
                ChangeCount = changeCount,
                Timestamp = DateTime.UtcNow
            };

            await _hubContext.Clients.Group("learners")
                .SendAsync("DictionaryUpdated", notification);

            _logger.LogInformation(
                "Notification de mise à jour du dictionnaire envoyée: version={Version}, changements={ChangeCount}",
                newVersion, changeCount);
        }

        public async Task<string> ComputeChecksumAsync()
        {
            // Calcul d'un checksum basé sur les IDs et orthographes triés
            var data = await _context.Words
                .AsNoTracking()
                .OrderBy(w => w.Id)
                .Select(w => $"{w.Id}:{w.Orthographe}")
                .ToListAsync();

            var concatenated = string.Join("|", data);
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(concatenated));
            var checksum = Convert.ToBase64String(hashBytes);

            // Mettre à jour le checksum dans les métadonnées
            var metadata = await _context.Set<DictionaryMetadata>().FirstOrDefaultAsync();
            if (metadata != null)
            {
                metadata.Checksum = checksum;
                await _context.SaveChangesAsync();
            }

            return checksum;
        }
    }
}
