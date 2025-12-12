using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace serveur.Hubs
{
    /// <summary>
    /// Hub SignalR pour la communication temps réel
    /// </summary>
    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> _logger;

        // Mapping userId -> liste de connectionIds (un utilisateur peut avoir plusieurs onglets)
        private static readonly ConcurrentDictionary<string, HashSet<string>> _userConnections = new();

        // Informations supplémentaires sur les utilisateurs (statut, date de connexion)
        private static readonly ConcurrentDictionary<string, UserInfo> _userInfos = new();

        public class UserInfo
        {
            public string UserId { get; set; }
            public string Status { get; set; } = "online";
            public DateTime ConnectedAt { get; set; } = DateTime.UtcNow;
        }

        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Enregistrer un utilisateur avec son identifiant
        /// Doit être appelé par le client après la connexion
        /// </summary>
        public async Task Register(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Tentative d'enregistrement avec un userId vide");
                return;
            }

            _userConnections.AddOrUpdate(
                userId,
                new HashSet<string> { Context.ConnectionId },
                (key, existingSet) =>
                {
                    lock (existingSet)
                    {
                        existingSet.Add(Context.ConnectionId);
                    }
                    return existingSet;
                });

            // Stocker le userId dans les items de la connexion pour le retrouver à la déconnexion
            Context.Items["UserId"] = userId;

            // Ajouter/mettre à jour les infos utilisateur
            _userInfos.AddOrUpdate(
                userId,
                new UserInfo { UserId = userId, Status = "online", ConnectedAt = DateTime.UtcNow },
                (key, existing) => existing);

            _logger.LogInformation("Utilisateur {UserId} enregistré avec ConnectionId {ConnectionId}",
                userId, Context.ConnectionId);

            // Notifier les autres que cet utilisateur est connecté
            await Clients.Others.SendAsync("UserConnected", userId);
        }

        /// <summary>
        /// Appelé quand un client se connecte
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("Client connecté: {ConnectionId}", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Appelé quand un client se déconnecte
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.Items["UserId"] as string;

            if (!string.IsNullOrEmpty(userId) && _userConnections.TryGetValue(userId, out var connections))
            {
                lock (connections)
                {
                    connections.Remove(Context.ConnectionId);
                    if (connections.Count == 0)
                    {
                        _userConnections.TryRemove(userId, out _);
                        _userInfos.TryRemove(userId, out _);
                    }
                }

                _logger.LogInformation("Client déconnecté: {ConnectionId}, UserId: {UserId}",
                    Context.ConnectionId, userId);

                await Clients.Others.SendAsync("UserDisconnected", userId);
            }
            else
            {
                _logger.LogInformation("Client déconnecté: {ConnectionId}", Context.ConnectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Obtenir les connectionIds d'un utilisateur
        /// </summary>
        private IReadOnlyList<string> GetConnectionIds(string userId)
        {
            if (_userConnections.TryGetValue(userId, out var connections))
            {
                lock (connections)
                {
                    return connections.ToList();
                }
            }
            return Array.Empty<string>();
        }

        /// <summary>
        /// Rejoindre un groupe (ex: salle d'examen, classe)
        /// </summary>
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation("Client {ConnectionId} a rejoint le groupe {GroupName}",
                Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("UserJoinedGroup",
                Context.UserIdentifier, groupName);
        }

        /// <summary>
        /// Quitter un groupe
        /// </summary>
        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation("Client {ConnectionId} a quitté le groupe {GroupName}",
                Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("UserLeftGroup",
                Context.UserIdentifier, groupName);
        }

        /// <summary>
        /// Envoyer une notification à un utilisateur spécifique
        /// </summary>
        public async Task SendNotification(string destinataireId, string message, string type = "info")
        {
            var expediteurId = Context.Items["UserId"] as string;

            var notification = new
            {
                Id = Guid.NewGuid(),
                ExpediteurId = expediteurId,
                Message = message,
                Type = type,
                Timestamp = DateTime.UtcNow
            };

            // Debug: afficher les utilisateurs enregistrés
            var registeredUsers = string.Join(", ", _userConnections.Keys);
            _logger.LogInformation("Utilisateurs enregistrés: [{Users}]", registeredUsers);

            var connectionIds = GetConnectionIds(destinataireId);
            if (connectionIds.Count > 0)
            {
                await Clients.Clients(connectionIds).SendAsync("ReceiveNotification", notification);
                _logger.LogInformation("Notification envoyée de {From} à {To} ({Count} connexions)",
                    expediteurId, destinataireId, connectionIds.Count);
            }
            else
            {
                _logger.LogWarning("Destinataire {DestinataireId} non trouvé ou non connecté. Utilisateurs disponibles: [{Users}]",
                    destinataireId, registeredUsers);
            }
        }

        /// <summary>
        /// Envoyer une alerte à tous les utilisateurs d'un groupe
        /// </summary>
        public async Task SendAlertToGroup(string groupName, string message, string niveau = "warning")
        {
            var expediteurId = Context.Items["UserId"] as string;

            var alerte = new
            {
                Id = Guid.NewGuid(),
                ExpediteurId = expediteurId,
                Message = message,
                Niveau = niveau,
                Timestamp = DateTime.UtcNow
            };

            await Clients.Group(groupName).SendAsync("ReceiveAlert", alerte);
            _logger.LogInformation("Alerte envoyée au groupe {GroupName} par {UserId}", groupName, expediteurId);
        }

        /// <summary>
        /// Envoyer une alerte à tous les utilisateurs connectés
        /// </summary>
        public async Task SendAlert(string message, string niveau = "warning")
        {
            var expediteurId = Context.Items["UserId"] as string;

            var alerte = new
            {
                Id = Guid.NewGuid(),
                ExpediteurId = expediteurId,
                Message = message,
                Niveau = niveau,
                Timestamp = DateTime.UtcNow
            };

            await Clients.All.SendAsync("ReceiveAlert", alerte);
            _logger.LogInformation("Alerte globale envoyée par {UserId}", expediteurId);
        }

        /// <summary>
        /// Mettre à jour son statut (en ligne, occupé, examen en cours, etc.)
        /// </summary>
        public async Task UpdateStatus(string status)
        {
            var userId = Context.Items["UserId"] as string;

            // Mettre à jour le statut dans les infos utilisateur
            if (!string.IsNullOrEmpty(userId) && _userInfos.TryGetValue(userId, out var userInfo))
            {
                userInfo.Status = status;
            }

            await Clients.Others.SendAsync("StatusUpdate", userId, status);
            _logger.LogInformation("Statut mis à jour pour {UserId}: {Status}", userId, status);
        }

        /// <summary>
        /// Envoyer un message à un groupe (chat/collaboration)
        /// </summary>
        public async Task SendMessageToGroup(string groupName, string message)
        {
            var expediteurId = Context.Items["UserId"] as string;

            var msg = new
            {
                Id = Guid.NewGuid(),
                ExpediteurId = expediteurId,
                Message = message,
                Timestamp = DateTime.UtcNow
            };

            await Clients.Group(groupName).SendAsync("ReceiveMessage", msg);
            _logger.LogInformation("Message envoyé au groupe {GroupName} par {UserId}", groupName, expediteurId);
        }

        /// <summary>
        /// Obtenir la liste des utilisateurs connectés avec leurs informations
        /// </summary>
        public Task<List<UserInfo>> GetConnectedUsers()
        {
            var users = _userInfos.Values.ToList();
            _logger.LogInformation("GetConnectedUsers appelé, {Count} utilisateurs connectés", users.Count);
            return Task.FromResult(users);
        }
    }
}
