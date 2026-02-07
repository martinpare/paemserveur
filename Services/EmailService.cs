using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace serveur.Services
{
    /// <summary>
    /// Service d'envoi de courriels avec template unifié
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        // Configuration par défaut
        private const string DEFAULT_SENDER_NAME_FR = "Mezur - Ne pas répondre";
        private const string DEFAULT_SENDER_NAME_EN = "Mezur - Do not reply";
        private const string DEFAULT_SENDER_EMAIL = "noreply@mezur.ca";

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<bool> SendEmailAsync(
            string to,
            string subject,
            string body,
            string recipientFirstName,
            string senderName = null,
            string senderEmail = null,
            string bcc = null,
            bool wrapWithTemplate = true,
            string language = "fr")
        {
            try
            {
                var defaultSenderName = language == "en" ? DEFAULT_SENDER_NAME_EN : DEFAULT_SENDER_NAME_FR;
                var finalSenderName = senderName ?? _configuration["Email:SenderName"] ?? defaultSenderName;
                var finalSenderEmail = senderEmail ?? _configuration["Email:SenderEmail"] ?? DEFAULT_SENDER_EMAIL;
                var finalBody = wrapWithTemplate ? WrapWithTemplate(body, recipientFirstName, language) : body;

                var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
                var smtpUser = _configuration["Email:SmtpUser"];
                var smtpPassword = _configuration["Email:SmtpPassword"];
                var enableSsl = bool.Parse(_configuration["Email:EnableSsl"] ?? "true");

                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    EnableSsl = enableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                // Configurer les credentials si fournis
                if (!string.IsNullOrEmpty(smtpUser) && !string.IsNullOrEmpty(smtpPassword))
                {
                    client.Credentials = new NetworkCredential(smtpUser, smtpPassword);
                }

                using var message = new MailMessage
                {
                    From = new MailAddress(finalSenderEmail, finalSenderName),
                    Subject = subject,
                    Body = finalBody,
                    IsBodyHtml = true
                };

                message.To.Add(to);

                if (!string.IsNullOrEmpty(bcc))
                {
                    message.Bcc.Add(bcc);
                }

                await client.SendMailAsync(message);

                _logger.LogInformation("Courriel envoyé avec succès à {To}", to);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'envoi du courriel à {To}", to);
                return false;
            }
        }

        /// <inheritdoc />
        public string WrapWithTemplate(string content, string recipientFirstName, string language = "fr")
        {
            var year = DateTime.Now.Year;
            var isEnglish = language == "en";

            var greeting = isEnglish
                ? $"Hello {recipientFirstName},"
                : $"Bonjour {recipientFirstName},";

            var subtitle = isEnglish
                ? "Online assessment administration platform"
                : "Plateforme d'administration d'épreuves en ligne";

            var footerMessage = isEnglish
                ? "This email was sent automatically by the Mezur system.<br>Please do not reply directly to this message."
                : "Ce courriel a été envoyé automatiquement par le système Mezur.<br>Veuillez ne pas répondre directement à ce message.";

            var copyright = isEnglish
                ? $"© {year} Softinov - Software Innovations Inc. All rights reserved."
                : $"© {year} Softinov - Innovations logicielles Inc. Tous droits réservés.";

            var htmlLang = isEnglish ? "en" : "fr";

            return $@"
<!DOCTYPE html>
<html lang=""{htmlLang}"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
</head>
<body style=""margin: 0; padding: 0; background-color: #ffffff; font-family: Arial, sans-serif;"">
    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color: #ffffff;"">
        <tr>
            <td align=""center"" style=""padding: 20px 0;"">
                <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""background-color: #ffffff; overflow: hidden;"">
                    <!-- HEADER -->
                    <tr>
                        <td>
                            <div style=""padding: 10px 20px; text-align: center; border-bottom: 1px solid #e0e0e0;"">
                                <h1 style=""color: #333333; margin: 0; font-family: Arial, sans-serif; font-size: 24px; font-weight: normal;"">
                                    Mezur
                                </h1>
                                <p style=""color: #666666; margin: 8px 0 0 0; font-family: Arial, sans-serif; font-size: 14px;"">
                                    {subtitle}
                                </p>
                            </div>
                        </td>
                    </tr>
                    <!-- CONTENT -->
                    <tr>
                        <td style=""padding: 30px;"">
                            {greeting}<br><br>
                            {content}
                        </td>
                    </tr>
                    <!-- FOOTER -->
                    <tr>
                        <td>
                            <div style=""padding: 20px; text-align: center; border-top: 1px solid #e0e0e0;"">
                                <p style=""color: #757575; margin: 0 0 10px 0; font-family: Arial, sans-serif; font-size: 12px;"">
                                    {footerMessage}
                                </p>
                                <p style=""color: #9e9e9e; margin: 0; font-family: Arial, sans-serif; font-size: 11px;"">
                                    {copyright}
                                </p>
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
        }
    }
}
