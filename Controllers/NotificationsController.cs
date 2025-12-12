using Microsoft.AspNetCore.Mvc;
using serveur.Services;
using System.Threading.Tasks;

namespace serveur.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Envoyer une notification à un utilisateur spécifique
        /// </summary>
        [HttpPost("utilisateur/{userId}")]
        public async Task<IActionResult> EnvoyerNotification(string userId, [FromBody] NotificationRequest request)
        {
            await _notificationService.SendToUserAsync(userId, request.Message, request.Type ?? "info");
            return Ok(new { success = true, message = "Notification envoyée" });
        }

        /// <summary>
        /// Envoyer une alerte à un groupe
        /// </summary>
        [HttpPost("groupe/{groupName}")]
        public async Task<IActionResult> EnvoyerAlerteGroupe(string groupName, [FromBody] AlerteRequest request)
        {
            await _notificationService.SendToGroupAsync(groupName, request.Message, request.Niveau ?? "info");
            return Ok(new { success = true, message = $"Alerte envoyée au groupe {groupName}" });
        }

        /// <summary>
        /// Envoyer une alerte globale à tous les utilisateurs connectés
        /// </summary>
        [HttpPost("globale")]
        public async Task<IActionResult> EnvoyerAlerteGlobale([FromBody] AlerteRequest request)
        {
            await _notificationService.SendToAllAsync(request.Message, request.Niveau ?? "warning");
            return Ok(new { success = true, message = "Alerte globale envoyée" });
        }

        /// <summary>
        /// Mettre à jour le statut d'un utilisateur
        /// </summary>
        [HttpPost("statut/{userId}")]
        public async Task<IActionResult> MettreAJourStatut(string userId, [FromBody] StatutRequest request)
        {
            await _notificationService.NotifyStatusChangeAsync(userId, request.Status);
            return Ok(new { success = true, message = "Statut mis à jour" });
        }
    }

    public class NotificationRequest
    {
        public string Message { get; set; }
        public string Type { get; set; }
    }

    public class AlerteRequest
    {
        public string Message { get; set; }
        public string Niveau { get; set; }
    }

    public class StatutRequest
    {
        public string Status { get; set; }
    }
}
