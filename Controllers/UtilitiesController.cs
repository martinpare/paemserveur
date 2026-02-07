using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using serveur.Data;
using serveur.Models.Dtos;
using serveur.Services;

namespace serveur.Controllers
{
    /// <summary>
    /// Contrôleur pour les utilitaires (envoi de courriels, etc.)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UtilitiesController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<UtilitiesController> _logger;
        private readonly AppDbContext _context;

        public UtilitiesController(IEmailService emailService, ILogger<UtilitiesController> logger, AppDbContext context)
        {
            _emailService = emailService;
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Envoie un courriel
        /// </summary>
        /// <param name="mailDto">Informations du courriel à envoyer</param>
        /// <returns>Résultat de l'envoi</returns>
        [HttpPost("SendMail")]
        [AllowAnonymous]
        public async Task<ActionResult<SendMailResponseDto>> SendMail([FromBody] SendMailDto mailDto)
        {
            try
            {
                _logger.LogInformation("Demande d'envoi de courriel à {To}", mailDto.Mail);

                // Récupérer le prénom du destinataire s'il n'est pas fourni
                var recipientFirstName = mailDto.RecipientFirstName;
                if (string.IsNullOrWhiteSpace(recipientFirstName))
                {
                    var user = await _context.Users
                        .Where(u => u.Mail == mailDto.Mail)
                        .Select(u => new { u.FirstName })
                        .FirstOrDefaultAsync();

                    recipientFirstName = user?.FirstName ?? "";
                }

                var success = await _emailService.SendEmailAsync(
                    to: mailDto.Mail,
                    subject: mailDto.Subject,
                    body: mailDto.Body,
                    recipientFirstName: recipientFirstName,
                    senderName: mailDto.SenderName,
                    senderEmail: mailDto.SenderMail,
                    bcc: mailDto.BccMail,
                    wrapWithTemplate: mailDto.WrapWithTemplate,
                    language: mailDto.Language);

                if (success)
                {
                    return Ok(new SendMailResponseDto
                    {
                        Success = true,
                        Message = "Courriel envoyé avec succès"
                    });
                }
                else
                {
                    return StatusCode(500, new SendMailResponseDto
                    {
                        Success = false,
                        Message = "Erreur lors de l'envoi du courriel"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'envoi du courriel");
                return StatusCode(500, new SendMailResponseDto
                {
                    Success = false,
                    Message = "Erreur interne du serveur"
                });
            }
        }
    }
}
