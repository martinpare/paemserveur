using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using serveur.Data;
using serveur.Models.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace serveur.Hubs
{
    #region État en mémoire

    public class ExamSessionState
    {
        public int Id { get; set; }
        public string GroupCode { get; set; }
        public int SessionId { get; set; }
        public int SupervisorUserId { get; set; }
        public string SupervisorConnectionId { get; set; }
        public string Status { get; set; } = "pending";
        public DateTime? StartedAt { get; set; }
        public ConcurrentDictionary<int, ParticipantState> Participants { get; set; } = new();
    }

    public class ParticipantState
    {
        public int Id { get; set; }
        public int LearnerId { get; set; }
        public string LearnerName { get; set; }
        public string PermanentCode { get; set; }
        public string ConnectionId { get; set; }
        public string Status { get; set; } = "pending";
        public int RemainingTimeSeconds { get; set; }
        public int AdditionalTimeMinutes { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime LastHeartbeat { get; set; }
        public bool ChatEnabled { get; set; } = true;
    }

    #endregion

    public class ExamHub : Hub
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ExamHub> _logger;

        // Sessions actives indexées par groupCode
        private static readonly ConcurrentDictionary<string, ExamSessionState> _activeSessions = new();

        // Mapping connectionId → (examSessionId, participantId ou null si superviseur)
        private static readonly ConcurrentDictionary<string, (int ExamSessionId, int? ParticipantId)> _connectionMappings = new();

        public ExamHub(AppDbContext context, ILogger<ExamHub> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Lifecycle

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("[ExamHub] Client connecté: {ConnectionId}", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connectionId = Context.ConnectionId;
            _logger.LogInformation("[ExamHub] Client déconnecté: {ConnectionId}", connectionId);

            if (_connectionMappings.TryRemove(connectionId, out var mapping))
            {
                var session = _activeSessions.Values.FirstOrDefault(s => s.Id == mapping.ExamSessionId);
                if (session != null)
                {
                    if (mapping.ParticipantId.HasValue)
                    {
                        // C'est un participant qui se déconnecte
                        if (session.Participants.TryGetValue(mapping.ParticipantId.Value, out var participant))
                        {
                            var previousStatus = participant.Status;
                            participant.Status = "disconnected";
                            participant.ConnectionId = null;

                            // Persister en DB
                            var dbParticipant = await _context.Set<ExamParticipant>()
                                .FirstOrDefaultAsync(p => p.Id == mapping.ParticipantId.Value);
                            if (dbParticipant != null)
                            {
                                dbParticipant.Status = "disconnected";
                                dbParticipant.ConnectionId = null;
                                dbParticipant.UpdatedAt = DateTime.UtcNow;
                            }

                            // Logger l'incident
                            _context.Set<ExamIncident>().Add(new ExamIncident
                            {
                                ExamParticipantId = mapping.ParticipantId.Value,
                                IncidentType = "disconnect",
                                Description = exception?.Message ?? "Déconnexion",
                                PreviousStatus = previousStatus,
                                CreatedAt = DateTime.UtcNow
                            });

                            await _context.SaveChangesAsync();

                            // Notifier le surveillant
                            if (!string.IsNullOrEmpty(session.SupervisorConnectionId))
                            {
                                await Clients.Client(session.SupervisorConnectionId).SendAsync("ParticipantStatusChanged", new
                                {
                                    participantId = mapping.ParticipantId.Value,
                                    learnerId = participant.LearnerId,
                                    newStatus = "disconnected",
                                    previousStatus = previousStatus
                                });
                            }

                            _logger.LogInformation("[ExamHub] Participant {ParticipantId} déconnecté de la session {GroupCode}",
                                mapping.ParticipantId.Value, session.GroupCode);
                        }
                    }
                    else
                    {
                        // C'est le surveillant qui se déconnecte
                        session.SupervisorConnectionId = null;
                        _logger.LogWarning("[ExamHub] Surveillant déconnecté de la session {GroupCode}", session.GroupCode);

                        // Notifier tous les participants
                        await Clients.Group($"exam_{session.GroupCode}").SendAsync("SupervisorDisconnected");
                    }
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        #endregion

        #region Méthodes Surveillant

        /// <summary>
        /// Créer une nouvelle session d'examen et obtenir le code de groupe
        /// </summary>
        public async Task<object> CreateExamSession(int sessionId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return new { success = false, error = "Authentification requise" };
                }

                // Générer un code de groupe unique
                var groupCode = GenerateGroupCode();
                while (await _context.Set<ExamSession>().AnyAsync(s => s.GroupCode == groupCode))
                {
                    groupCode = GenerateGroupCode();
                }

                // Créer en DB
                var examSession = new ExamSession
                {
                    SessionId = sessionId,
                    GroupCode = groupCode,
                    SupervisorUserId = userId.Value,
                    Status = "pending",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Set<ExamSession>().Add(examSession);
                await _context.SaveChangesAsync();

                // Créer l'état en mémoire
                var sessionState = new ExamSessionState
                {
                    Id = examSession.Id,
                    GroupCode = groupCode,
                    SessionId = sessionId,
                    SupervisorUserId = userId.Value,
                    SupervisorConnectionId = Context.ConnectionId,
                    Status = "pending"
                };

                _activeSessions[groupCode] = sessionState;
                _connectionMappings[Context.ConnectionId] = (examSession.Id, null);

                // Joindre le groupe SignalR
                await Groups.AddToGroupAsync(Context.ConnectionId, $"exam_{groupCode}");

                // Logger l'action
                await LogAction(examSession.Id, "session_created", "supervisor", userId.Value, new { sessionId, groupCode });

                _logger.LogInformation("[ExamHub] Session créée: {GroupCode} par utilisateur {UserId}", groupCode, userId.Value);

                return new { success = true, groupCode, examSessionId = examSession.Id };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ExamHub] Erreur lors de la création de session");
                return new { success = false, error = "Erreur lors de la création de la session" };
            }
        }

        /// <summary>
        /// Rejoindre une session existante en tant que surveillant
        /// </summary>
        public async Task<object> JoinAsSuperviser(string groupCode)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return new { success = false, error = "Authentification requise" };
                }

                var session = await GetOrLoadSession(groupCode);
                if (session == null)
                {
                    return new { success = false, error = "Session non trouvée" };
                }

                if (session.SupervisorUserId != userId.Value)
                {
                    return new { success = false, error = "Vous n'êtes pas le surveillant de cette session" };
                }

                session.SupervisorConnectionId = Context.ConnectionId;
                _connectionMappings[Context.ConnectionId] = (session.Id, null);

                await Groups.AddToGroupAsync(Context.ConnectionId, $"exam_{groupCode}");

                _logger.LogInformation("[ExamHub] Surveillant {UserId} rejoint la session {GroupCode}", userId.Value, groupCode);

                return new { success = true, session = GetSessionDto(session) };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ExamHub] Erreur lors du join surveillant");
                return new { success = false, error = "Erreur lors de la connexion" };
            }
        }

        /// <summary>
        /// Obtenir l'état complet de la session
        /// </summary>
        public async Task<object> GetSessionState(string groupCode)
        {
            try
            {
                var session = await GetOrLoadSession(groupCode);
                if (session == null)
                {
                    return new { success = false, error = "Session non trouvée" };
                }

                return new { success = true, session = GetSessionDto(session) };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ExamHub] Erreur lors de GetSessionState");
                return new { success = false, error = "Erreur lors de la récupération de l'état" };
            }
        }

        /// <summary>
        /// Démarrer l'examen pour tous ou certains participants
        /// </summary>
        public async Task<object> StartExam(string groupCode, int durationMinutes, int[] learnerIds = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                var session = await GetOrLoadSession(groupCode);

                if (session == null)
                    return new { success = false, error = "Session non trouvée" };

                if (session.SupervisorUserId != userId)
                    return new { success = false, error = "Non autorisé" };

                var now = DateTime.UtcNow;
                var durationSeconds = durationMinutes * 60;

                // Mettre à jour la session si c'est le premier démarrage
                if (session.Status == "pending")
                {
                    session.Status = "active";
                    session.StartedAt = now;

                    var dbSession = await _context.Set<ExamSession>().FindAsync(session.Id);
                    if (dbSession != null)
                    {
                        dbSession.Status = "active";
                        dbSession.StartedAt = now;
                        dbSession.UpdatedAt = now;
                    }
                }

                // Déterminer les participants à démarrer
                var participantsToStart = session.Participants.Values
                    .Where(p => (learnerIds == null || learnerIds.Contains(p.LearnerId)) &&
                                (p.Status == "pending" || p.Status == "connected"))
                    .ToList();

                foreach (var participant in participantsToStart)
                {
                    participant.Status = "in_exam";
                    participant.StartedAt = now;
                    participant.RemainingTimeSeconds = durationSeconds + (participant.AdditionalTimeMinutes * 60);

                    // Persister en DB
                    var dbParticipant = await _context.Set<ExamParticipant>().FindAsync(participant.Id);
                    if (dbParticipant != null)
                    {
                        dbParticipant.Status = "in_exam";
                        dbParticipant.StartedAt = now;
                        dbParticipant.RemainingTimeSeconds = participant.RemainingTimeSeconds;
                        dbParticipant.UpdatedAt = now;
                    }

                    // Notifier le participant
                    if (!string.IsNullOrEmpty(participant.ConnectionId))
                    {
                        await Clients.Client(participant.ConnectionId).SendAsync("ExamStarted", new
                        {
                            durationSeconds = participant.RemainingTimeSeconds,
                            startedAt = now
                        });
                    }
                }

                await _context.SaveChangesAsync();
                await LogAction(session.Id, "exam_started", "supervisor", userId.Value, new { learnerIds, durationMinutes });

                // Notifier le groupe
                await Clients.Group($"exam_{groupCode}").SendAsync("SessionStateChanged", GetSessionDto(session));

                _logger.LogInformation("[ExamHub] Examen démarré pour session {GroupCode}, {Count} participants",
                    groupCode, participantsToStart.Count);

                return new { success = true, startedCount = participantsToStart.Count };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ExamHub] Erreur lors du démarrage de l'examen");
                return new { success = false, error = "Erreur lors du démarrage" };
            }
        }

        /// <summary>
        /// Mettre l'examen en pause
        /// </summary>
        public async Task<object> PauseExam(string groupCode, int? learnerId = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                var session = await GetOrLoadSession(groupCode);

                if (session == null)
                    return new { success = false, error = "Session non trouvée" };

                if (session.SupervisorUserId != userId)
                    return new { success = false, error = "Non autorisé" };

                var now = DateTime.UtcNow;
                var pausedCount = 0;

                var participantsToPause = session.Participants.Values
                    .Where(p => (learnerId == null || p.LearnerId == learnerId) && p.Status == "in_exam")
                    .ToList();

                foreach (var participant in participantsToPause)
                {
                    participant.Status = "paused";

                    var dbParticipant = await _context.Set<ExamParticipant>().FindAsync(participant.Id);
                    if (dbParticipant != null)
                    {
                        dbParticipant.Status = "paused";
                        dbParticipant.RemainingTimeSeconds = participant.RemainingTimeSeconds;
                        dbParticipant.UpdatedAt = now;
                    }

                    if (!string.IsNullOrEmpty(participant.ConnectionId))
                    {
                        await Clients.Client(participant.ConnectionId).SendAsync("ExamPaused");
                    }

                    pausedCount++;
                }

                if (learnerId == null && participantsToPause.Count > 0)
                {
                    session.Status = "paused";
                    var dbSession = await _context.Set<ExamSession>().FindAsync(session.Id);
                    if (dbSession != null)
                    {
                        dbSession.Status = "paused";
                        dbSession.UpdatedAt = now;
                    }
                }

                await _context.SaveChangesAsync();
                await LogAction(session.Id, "exam_paused", "supervisor", userId.Value, new { learnerId, pausedCount });

                await Clients.Group($"exam_{groupCode}").SendAsync("SessionStateChanged", GetSessionDto(session));

                return new { success = true, pausedCount };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ExamHub] Erreur lors de la pause");
                return new { success = false, error = "Erreur lors de la pause" };
            }
        }

        /// <summary>
        /// Reprendre l'examen après une pause
        /// </summary>
        public async Task<object> ResumeExam(string groupCode, int? learnerId = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                var session = await GetOrLoadSession(groupCode);

                if (session == null)
                    return new { success = false, error = "Session non trouvée" };

                if (session.SupervisorUserId != userId)
                    return new { success = false, error = "Non autorisé" };

                var now = DateTime.UtcNow;
                var resumedCount = 0;

                var participantsToResume = session.Participants.Values
                    .Where(p => (learnerId == null || p.LearnerId == learnerId) && p.Status == "paused")
                    .ToList();

                foreach (var participant in participantsToResume)
                {
                    participant.Status = "in_exam";

                    var dbParticipant = await _context.Set<ExamParticipant>().FindAsync(participant.Id);
                    if (dbParticipant != null)
                    {
                        dbParticipant.Status = "in_exam";
                        dbParticipant.UpdatedAt = now;
                    }

                    if (!string.IsNullOrEmpty(participant.ConnectionId))
                    {
                        await Clients.Client(participant.ConnectionId).SendAsync("ExamResumed");
                    }

                    resumedCount++;
                }

                if (learnerId == null && session.Status == "paused")
                {
                    session.Status = "active";
                    var dbSession = await _context.Set<ExamSession>().FindAsync(session.Id);
                    if (dbSession != null)
                    {
                        dbSession.Status = "active";
                        dbSession.UpdatedAt = now;
                    }
                }

                await _context.SaveChangesAsync();
                await LogAction(session.Id, "exam_resumed", "supervisor", userId.Value, new { learnerId, resumedCount });

                await Clients.Group($"exam_{groupCode}").SendAsync("SessionStateChanged", GetSessionDto(session));

                return new { success = true, resumedCount };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ExamHub] Erreur lors de la reprise");
                return new { success = false, error = "Erreur lors de la reprise" };
            }
        }

        /// <summary>
        /// Terminer l'examen
        /// </summary>
        public async Task<object> EndExam(string groupCode, int? learnerId = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                var session = await GetOrLoadSession(groupCode);

                if (session == null)
                    return new { success = false, error = "Session non trouvée" };

                if (session.SupervisorUserId != userId)
                    return new { success = false, error = "Non autorisé" };

                var now = DateTime.UtcNow;
                var endedCount = 0;

                var participantsToEnd = session.Participants.Values
                    .Where(p => (learnerId == null || p.LearnerId == learnerId) &&
                                p.Status != "ended" && p.Status != "submitted")
                    .ToList();

                foreach (var participant in participantsToEnd)
                {
                    participant.Status = "ended";

                    var dbParticipant = await _context.Set<ExamParticipant>().FindAsync(participant.Id);
                    if (dbParticipant != null)
                    {
                        dbParticipant.Status = "ended";
                        dbParticipant.UpdatedAt = now;
                    }

                    if (!string.IsNullOrEmpty(participant.ConnectionId))
                    {
                        await Clients.Client(participant.ConnectionId).SendAsync("ExamEnded");
                    }

                    endedCount++;
                }

                if (learnerId == null)
                {
                    session.Status = "ended";
                    var dbSession = await _context.Set<ExamSession>().FindAsync(session.Id);
                    if (dbSession != null)
                    {
                        dbSession.Status = "ended";
                        dbSession.EndedAt = now;
                        dbSession.UpdatedAt = now;
                    }
                }

                await _context.SaveChangesAsync();
                await LogAction(session.Id, "exam_ended", "supervisor", userId.Value, new { learnerId, endedCount });

                await Clients.Group($"exam_{groupCode}").SendAsync("SessionStateChanged", GetSessionDto(session));

                return new { success = true, endedCount };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ExamHub] Erreur lors de la fin de l'examen");
                return new { success = false, error = "Erreur lors de la terminaison" };
            }
        }

        /// <summary>
        /// Ajouter du temps supplémentaire
        /// </summary>
        public async Task<object> AddTime(string groupCode, int minutes, int? learnerId = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                var session = await GetOrLoadSession(groupCode);

                if (session == null)
                    return new { success = false, error = "Session non trouvée" };

                if (session.SupervisorUserId != userId)
                    return new { success = false, error = "Non autorisé" };

                var now = DateTime.UtcNow;
                var additionalSeconds = minutes * 60;
                var updatedCount = 0;

                var participantsToUpdate = session.Participants.Values
                    .Where(p => (learnerId == null || p.LearnerId == learnerId) &&
                                (p.Status == "in_exam" || p.Status == "paused"))
                    .ToList();

                foreach (var participant in participantsToUpdate)
                {
                    participant.RemainingTimeSeconds += additionalSeconds;
                    participant.AdditionalTimeMinutes += minutes;

                    var dbParticipant = await _context.Set<ExamParticipant>().FindAsync(participant.Id);
                    if (dbParticipant != null)
                    {
                        dbParticipant.RemainingTimeSeconds = participant.RemainingTimeSeconds;
                        dbParticipant.AdditionalTimeMinutes = participant.AdditionalTimeMinutes;
                        dbParticipant.UpdatedAt = now;
                    }

                    if (!string.IsNullOrEmpty(participant.ConnectionId))
                    {
                        await Clients.Client(participant.ConnectionId).SendAsync("TimeAdded", new
                        {
                            minutes,
                            newRemainingSeconds = participant.RemainingTimeSeconds
                        });
                    }

                    updatedCount++;
                }

                await _context.SaveChangesAsync();
                await LogAction(session.Id, "time_added", "supervisor", userId.Value, new { learnerId, minutes, updatedCount });

                await Clients.Group($"exam_{groupCode}").SendAsync("SessionStateChanged", GetSessionDto(session));

                return new { success = true, updatedCount };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ExamHub] Erreur lors de l'ajout de temps");
                return new { success = false, error = "Erreur lors de l'ajout de temps" };
            }
        }

        /// <summary>
        /// Envoyer un message à un participant ou à tous
        /// </summary>
        public async Task<object> SendMessage(string groupCode, string message, int? learnerId = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                var session = await GetOrLoadSession(groupCode);

                if (session == null)
                    return new { success = false, error = "Session non trouvée" };

                if (session.SupervisorUserId != userId)
                    return new { success = false, error = "Non autorisé" };

                var now = DateTime.UtcNow;

                // Persister le message
                var examMessage = new ExamMessage
                {
                    ExamSessionId = session.Id,
                    SenderType = "supervisor",
                    SenderId = userId.Value,
                    RecipientType = learnerId.HasValue ? "learner" : null,
                    RecipientId = learnerId,
                    MessageText = message,
                    IsRead = false,
                    CreatedAt = now
                };

                _context.Set<ExamMessage>().Add(examMessage);
                await _context.SaveChangesAsync();

                var messageDto = new
                {
                    id = examMessage.Id,
                    senderType = "supervisor",
                    senderId = userId.Value,
                    message = message,
                    timestamp = now
                };

                if (learnerId.HasValue)
                {
                    // Message à un participant spécifique
                    var participant = session.Participants.Values.FirstOrDefault(p => p.LearnerId == learnerId);
                    if (participant != null && !string.IsNullOrEmpty(participant.ConnectionId))
                    {
                        await Clients.Client(participant.ConnectionId).SendAsync("MessageReceived", messageDto);
                    }
                }
                else
                {
                    // Broadcast à tous
                    await Clients.Group($"exam_{groupCode}").SendAsync("MessageReceived", messageDto);
                }

                return new { success = true, messageId = examMessage.Id };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ExamHub] Erreur lors de l'envoi de message");
                return new { success = false, error = "Erreur lors de l'envoi" };
            }
        }

        /// <summary>
        /// Activer/désactiver le chat pour un participant
        /// </summary>
        public async Task<object> ToggleChat(string groupCode, int learnerId, bool enabled)
        {
            try
            {
                var userId = GetCurrentUserId();
                var session = await GetOrLoadSession(groupCode);

                if (session == null)
                    return new { success = false, error = "Session non trouvée" };

                if (session.SupervisorUserId != userId)
                    return new { success = false, error = "Non autorisé" };

                var participant = session.Participants.Values.FirstOrDefault(p => p.LearnerId == learnerId);
                if (participant == null)
                    return new { success = false, error = "Participant non trouvé" };

                participant.ChatEnabled = enabled;

                if (!string.IsNullOrEmpty(participant.ConnectionId))
                {
                    await Clients.Client(participant.ConnectionId).SendAsync("ChatToggled", enabled);
                }

                await LogAction(session.Id, enabled ? "chat_enabled" : "chat_disabled", "supervisor", userId.Value, new { learnerId });

                return new { success = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ExamHub] Erreur lors du toggle chat");
                return new { success = false, error = "Erreur" };
            }
        }

        /// <summary>
        /// Approuver une demande de pause
        /// </summary>
        public async Task<object> ApprovePause(string groupCode, int learnerId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var session = await GetOrLoadSession(groupCode);

                if (session == null)
                    return new { success = false, error = "Session non trouvée" };

                if (session.SupervisorUserId != userId)
                    return new { success = false, error = "Non autorisé" };

                var participant = session.Participants.Values.FirstOrDefault(p => p.LearnerId == learnerId);
                if (participant == null || participant.Status != "pause_requested")
                    return new { success = false, error = "Aucune demande de pause en attente" };

                participant.Status = "paused";

                var dbParticipant = await _context.Set<ExamParticipant>().FindAsync(participant.Id);
                if (dbParticipant != null)
                {
                    dbParticipant.Status = "paused";
                    dbParticipant.RemainingTimeSeconds = participant.RemainingTimeSeconds;
                    dbParticipant.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                if (!string.IsNullOrEmpty(participant.ConnectionId))
                {
                    await Clients.Client(participant.ConnectionId).SendAsync("PauseApproved");
                }

                await LogAction(session.Id, "pause_approved", "supervisor", userId.Value, new { learnerId });

                return new { success = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ExamHub] Erreur lors de l'approbation de pause");
                return new { success = false, error = "Erreur" };
            }
        }

        /// <summary>
        /// Refuser une demande de pause
        /// </summary>
        public async Task<object> DenyPause(string groupCode, int learnerId, string reason = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                var session = await GetOrLoadSession(groupCode);

                if (session == null)
                    return new { success = false, error = "Session non trouvée" };

                if (session.SupervisorUserId != userId)
                    return new { success = false, error = "Non autorisé" };

                var participant = session.Participants.Values.FirstOrDefault(p => p.LearnerId == learnerId);
                if (participant == null || participant.Status != "pause_requested")
                    return new { success = false, error = "Aucune demande de pause en attente" };

                participant.Status = "in_exam";

                var dbParticipant = await _context.Set<ExamParticipant>().FindAsync(participant.Id);
                if (dbParticipant != null)
                {
                    dbParticipant.Status = "in_exam";
                    dbParticipant.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                if (!string.IsNullOrEmpty(participant.ConnectionId))
                {
                    await Clients.Client(participant.ConnectionId).SendAsync("PauseDenied", reason);
                }

                await LogAction(session.Id, "pause_denied", "supervisor", userId.Value, new { learnerId, reason });

                return new { success = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ExamHub] Erreur lors du refus de pause");
                return new { success = false, error = "Erreur" };
            }
        }

        #endregion

        #region Méthodes Étudiant

        /// <summary>
        /// Rejoindre une session d'examen en tant qu'étudiant
        /// </summary>
        public async Task<object> JoinExam(string groupCode, int learnerId, string learnerName, string permanentCode)
        {
            try
            {
                var session = await GetOrLoadSession(groupCode);
                if (session == null)
                {
                    return new { success = false, error = "Code de groupe invalide" };
                }

                if (session.Status == "ended")
                {
                    return new { success = false, error = "Cette session est terminée" };
                }

                // Chercher ou créer le participant
                var participant = session.Participants.Values.FirstOrDefault(p => p.LearnerId == learnerId);

                if (participant == null)
                {
                    // Créer le participant en DB
                    var dbParticipant = new ExamParticipant
                    {
                        ExamSessionId = session.Id,
                        LearnerId = learnerId,
                        Status = "connected",
                        ConnectionId = Context.ConnectionId,
                        AdditionalTimeMinutes = 0,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.Set<ExamParticipant>().Add(dbParticipant);
                    await _context.SaveChangesAsync();

                    participant = new ParticipantState
                    {
                        Id = dbParticipant.Id,
                        LearnerId = learnerId,
                        LearnerName = learnerName,
                        PermanentCode = permanentCode,
                        ConnectionId = Context.ConnectionId,
                        Status = "connected",
                        ChatEnabled = true,
                        LastHeartbeat = DateTime.UtcNow
                    };

                    session.Participants[dbParticipant.Id] = participant;
                }
                else
                {
                    // Reconnexion
                    var previousStatus = participant.Status;
                    participant.ConnectionId = Context.ConnectionId;
                    participant.LastHeartbeat = DateTime.UtcNow;

                    if (participant.Status == "disconnected")
                    {
                        participant.Status = previousStatus == "in_exam" ? "in_exam" : "connected";
                    }

                    var dbParticipant = await _context.Set<ExamParticipant>().FindAsync(participant.Id);
                    if (dbParticipant != null)
                    {
                        dbParticipant.ConnectionId = Context.ConnectionId;
                        dbParticipant.Status = participant.Status;
                        dbParticipant.UpdatedAt = DateTime.UtcNow;
                        await _context.SaveChangesAsync();
                    }
                }

                _connectionMappings[Context.ConnectionId] = (session.Id, participant.Id);

                // Joindre le groupe SignalR
                await Groups.AddToGroupAsync(Context.ConnectionId, $"exam_{groupCode}");

                // Notifier le surveillant
                if (!string.IsNullOrEmpty(session.SupervisorConnectionId))
                {
                    await Clients.Client(session.SupervisorConnectionId).SendAsync("ParticipantJoined", new
                    {
                        participantId = participant.Id,
                        learnerId = participant.LearnerId,
                        learnerName = participant.LearnerName,
                        permanentCode = participant.PermanentCode,
                        status = participant.Status
                    });
                }

                await LogAction(session.Id, "participant_joined", "learner", learnerId, new { participantId = participant.Id });

                _logger.LogInformation("[ExamHub] Participant {LearnerId} rejoint la session {GroupCode}", learnerId, groupCode);

                return new
                {
                    success = true,
                    participantId = participant.Id,
                    sessionStatus = session.Status,
                    participantStatus = participant.Status,
                    remainingTimeSeconds = participant.RemainingTimeSeconds,
                    chatEnabled = participant.ChatEnabled
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ExamHub] Erreur lors du join étudiant");
                return new { success = false, error = "Erreur lors de la connexion" };
            }
        }

        /// <summary>
        /// Demander une pause
        /// </summary>
        public async Task<object> RequestPause(string groupCode, string reason)
        {
            try
            {
                var (session, participant) = await GetCurrentParticipant(groupCode);
                if (session == null || participant == null)
                    return new { success = false, error = "Session ou participant non trouvé" };

                if (participant.Status != "in_exam")
                    return new { success = false, error = "Vous n'êtes pas en examen" };

                participant.Status = "pause_requested";

                var dbParticipant = await _context.Set<ExamParticipant>().FindAsync(participant.Id);
                if (dbParticipant != null)
                {
                    dbParticipant.Status = "pause_requested";
                    dbParticipant.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                // Notifier le surveillant
                if (!string.IsNullOrEmpty(session.SupervisorConnectionId))
                {
                    await Clients.Client(session.SupervisorConnectionId).SendAsync("PauseRequested", new
                    {
                        participantId = participant.Id,
                        learnerId = participant.LearnerId,
                        learnerName = participant.LearnerName,
                        reason
                    });
                }

                await LogAction(session.Id, "pause_requested", "learner", participant.LearnerId, new { reason });

                return new { success = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ExamHub] Erreur lors de la demande de pause");
                return new { success = false, error = "Erreur" };
            }
        }

        /// <summary>
        /// Signaler un problème technique
        /// </summary>
        public async Task<object> ReportProblem(string groupCode, string description)
        {
            try
            {
                var (session, participant) = await GetCurrentParticipant(groupCode);
                if (session == null || participant == null)
                    return new { success = false, error = "Session ou participant non trouvé" };

                // Créer l'incident
                var incident = new ExamIncident
                {
                    ExamParticipantId = participant.Id,
                    IncidentType = "problem_reported",
                    Description = description,
                    PreviousStatus = participant.Status,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Set<ExamIncident>().Add(incident);
                await _context.SaveChangesAsync();

                // Notifier le surveillant
                if (!string.IsNullOrEmpty(session.SupervisorConnectionId))
                {
                    await Clients.Client(session.SupervisorConnectionId).SendAsync("IncidentReported", new
                    {
                        participantId = participant.Id,
                        learnerId = participant.LearnerId,
                        learnerName = participant.LearnerName,
                        incidentType = "problem_reported",
                        description,
                        timestamp = incident.CreatedAt
                    });
                }

                return new { success = true, incidentId = incident.Id };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ExamHub] Erreur lors du signalement de problème");
                return new { success = false, error = "Erreur" };
            }
        }

        /// <summary>
        /// Signaler un incident (sortie plein écran, changement d'onglet, etc.)
        /// </summary>
        public async Task<object> ReportIncident(string groupCode, string incidentType, string description = null)
        {
            try
            {
                var (session, participant) = await GetCurrentParticipant(groupCode);
                if (session == null || participant == null)
                    return new { success = false, error = "Session ou participant non trouvé" };

                var incident = new ExamIncident
                {
                    ExamParticipantId = participant.Id,
                    IncidentType = incidentType,
                    Description = description,
                    PreviousStatus = participant.Status,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Set<ExamIncident>().Add(incident);
                await _context.SaveChangesAsync();

                // Notifier le surveillant
                if (!string.IsNullOrEmpty(session.SupervisorConnectionId))
                {
                    await Clients.Client(session.SupervisorConnectionId).SendAsync("IncidentReported", new
                    {
                        participantId = participant.Id,
                        learnerId = participant.LearnerId,
                        learnerName = participant.LearnerName,
                        incidentType,
                        description,
                        timestamp = incident.CreatedAt
                    });
                }

                return new { success = true, incidentId = incident.Id };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ExamHub] Erreur lors du signalement d'incident");
                return new { success = false, error = "Erreur" };
            }
        }

        /// <summary>
        /// Soumettre l'examen
        /// </summary>
        public async Task<object> SubmitExam(string groupCode)
        {
            try
            {
                var (session, participant) = await GetCurrentParticipant(groupCode);
                if (session == null || participant == null)
                    return new { success = false, error = "Session ou participant non trouvé" };

                if (participant.Status != "in_exam" && participant.Status != "paused")
                    return new { success = false, error = "Vous n'êtes pas en examen" };

                var now = DateTime.UtcNow;
                participant.Status = "submitted";

                var dbParticipant = await _context.Set<ExamParticipant>().FindAsync(participant.Id);
                if (dbParticipant != null)
                {
                    dbParticipant.Status = "submitted";
                    dbParticipant.SubmittedAt = now;
                    dbParticipant.UpdatedAt = now;
                }

                await _context.SaveChangesAsync();
                await LogAction(session.Id, "exam_submitted", "learner", participant.LearnerId, null);

                // Notifier le surveillant
                if (!string.IsNullOrEmpty(session.SupervisorConnectionId))
                {
                    await Clients.Client(session.SupervisorConnectionId).SendAsync("ExamSubmitted", new
                    {
                        participantId = participant.Id,
                        learnerId = participant.LearnerId,
                        learnerName = participant.LearnerName,
                        submittedAt = now
                    });
                }

                _logger.LogInformation("[ExamHub] Examen soumis par {LearnerId} dans session {GroupCode}",
                    participant.LearnerId, groupCode);

                return new { success = true, submittedAt = now };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ExamHub] Erreur lors de la soumission");
                return new { success = false, error = "Erreur lors de la soumission" };
            }
        }

        /// <summary>
        /// Envoyer un message au surveillant
        /// </summary>
        public async Task<object> SendChatMessage(string groupCode, string message)
        {
            try
            {
                var (session, participant) = await GetCurrentParticipant(groupCode);
                if (session == null || participant == null)
                    return new { success = false, error = "Session ou participant non trouvé" };

                if (!participant.ChatEnabled)
                    return new { success = false, error = "Le chat est désactivé" };

                var now = DateTime.UtcNow;

                var examMessage = new ExamMessage
                {
                    ExamSessionId = session.Id,
                    SenderType = "learner",
                    SenderId = participant.LearnerId,
                    RecipientType = "supervisor",
                    RecipientId = session.SupervisorUserId,
                    MessageText = message,
                    IsRead = false,
                    CreatedAt = now
                };

                _context.Set<ExamMessage>().Add(examMessage);
                await _context.SaveChangesAsync();

                // Notifier le surveillant
                if (!string.IsNullOrEmpty(session.SupervisorConnectionId))
                {
                    await Clients.Client(session.SupervisorConnectionId).SendAsync("MessageReceived", new
                    {
                        id = examMessage.Id,
                        senderType = "learner",
                        senderId = participant.LearnerId,
                        senderName = participant.LearnerName,
                        participantId = participant.Id,
                        message,
                        timestamp = now
                    });
                }

                return new { success = true, messageId = examMessage.Id };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ExamHub] Erreur lors de l'envoi de message chat");
                return new { success = false, error = "Erreur lors de l'envoi" };
            }
        }

        /// <summary>
        /// Signal de vie périodique avec mise à jour du temps restant
        /// </summary>
        public async Task<object> Heartbeat(string groupCode, int remainingTimeSeconds)
        {
            try
            {
                var (session, participant) = await GetCurrentParticipant(groupCode);
                if (session == null || participant == null)
                    return new { success = false };

                participant.LastHeartbeat = DateTime.UtcNow;
                participant.RemainingTimeSeconds = remainingTimeSeconds;

                // Vérifier si le temps est écoulé
                if (remainingTimeSeconds <= 0 && participant.Status == "in_exam")
                {
                    participant.Status = "time_expired";

                    var dbParticipant = await _context.Set<ExamParticipant>().FindAsync(participant.Id);
                    if (dbParticipant != null)
                    {
                        dbParticipant.Status = "time_expired";
                        dbParticipant.RemainingTimeSeconds = 0;
                        dbParticipant.UpdatedAt = DateTime.UtcNow;
                        await _context.SaveChangesAsync();
                    }

                    await Clients.Caller.SendAsync("ForceSubmit");

                    if (!string.IsNullOrEmpty(session.SupervisorConnectionId))
                    {
                        await Clients.Client(session.SupervisorConnectionId).SendAsync("ParticipantStatusChanged", new
                        {
                            participantId = participant.Id,
                            learnerId = participant.LearnerId,
                            newStatus = "time_expired",
                            previousStatus = "in_exam"
                        });
                    }
                }

                return new { success = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ExamHub] Erreur heartbeat");
                return new { success = false };
            }
        }

        #endregion

        #region Helpers privés

        private int? GetCurrentUserId()
        {
            var userIdClaim = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out var userId))
                return userId;
            return null;
        }

        private async Task<ExamSessionState> GetOrLoadSession(string groupCode)
        {
            if (_activeSessions.TryGetValue(groupCode, out var session))
                return session;

            // Charger depuis la DB
            var dbSession = await _context.Set<ExamSession>()
                .Include(s => s.Participants)
                    .ThenInclude(p => p.Learner)
                .FirstOrDefaultAsync(s => s.GroupCode == groupCode);

            if (dbSession == null)
                return null;

            session = new ExamSessionState
            {
                Id = dbSession.Id,
                GroupCode = dbSession.GroupCode,
                SessionId = dbSession.SessionId,
                SupervisorUserId = dbSession.SupervisorUserId,
                Status = dbSession.Status,
                StartedAt = dbSession.StartedAt
            };

            foreach (var p in dbSession.Participants)
            {
                session.Participants[p.Id] = new ParticipantState
                {
                    Id = p.Id,
                    LearnerId = p.LearnerId,
                    LearnerName = p.Learner != null ? $"{p.Learner.FirstName} {p.Learner.LastName}" : "Inconnu",
                    PermanentCode = p.Learner?.PermanentCode,
                    ConnectionId = p.ConnectionId,
                    Status = p.Status,
                    RemainingTimeSeconds = p.RemainingTimeSeconds ?? 0,
                    AdditionalTimeMinutes = p.AdditionalTimeMinutes,
                    StartedAt = p.StartedAt,
                    ChatEnabled = true
                };
            }

            _activeSessions[groupCode] = session;
            return session;
        }

        private async Task<(ExamSessionState, ParticipantState)> GetCurrentParticipant(string groupCode)
        {
            var session = await GetOrLoadSession(groupCode);
            if (session == null)
                return (null, null);

            if (_connectionMappings.TryGetValue(Context.ConnectionId, out var mapping) && mapping.ParticipantId.HasValue)
            {
                if (session.Participants.TryGetValue(mapping.ParticipantId.Value, out var participant))
                    return (session, participant);
            }

            return (session, null);
        }

        private async Task LogAction(int examSessionId, string action, string actorType, int? actorId, object details)
        {
            var log = new ExamLog
            {
                ExamSessionId = examSessionId,
                Action = action,
                ActorType = actorType,
                ActorId = actorId,
                Details = details != null ? JsonSerializer.Serialize(details) : null,
                CreatedAt = DateTime.UtcNow
            };

            _context.Set<ExamLog>().Add(log);
            await _context.SaveChangesAsync();
        }

        private string GenerateGroupCode()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private object GetSessionDto(ExamSessionState session)
        {
            return new
            {
                id = session.Id,
                groupCode = session.GroupCode,
                sessionId = session.SessionId,
                supervisorUserId = session.SupervisorUserId,
                status = session.Status,
                startedAt = session.StartedAt,
                participants = session.Participants.Values.Select(p => new
                {
                    id = p.Id,
                    learnerId = p.LearnerId,
                    learnerName = p.LearnerName,
                    permanentCode = p.PermanentCode,
                    status = p.Status,
                    remainingTimeSeconds = p.RemainingTimeSeconds,
                    additionalTimeMinutes = p.AdditionalTimeMinutes,
                    startedAt = p.StartedAt,
                    isConnected = !string.IsNullOrEmpty(p.ConnectionId),
                    chatEnabled = p.ChatEnabled,
                    lastHeartbeat = p.LastHeartbeat
                }).ToList()
            };
        }

        #endregion
    }
}
