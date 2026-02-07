using System.Threading.Tasks;

namespace serveur.Services
{
    /// <summary>
    /// Interface pour le service d'envoi de courriels
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Envoie un courriel
        /// </summary>
        /// <param name="to">Adresse du destinataire</param>
        /// <param name="subject">Sujet du courriel</param>
        /// <param name="body">Corps du courriel (HTML)</param>
        /// <param name="recipientFirstName">Prénom du destinataire pour la salutation</param>
        /// <param name="senderName">Nom de l'expéditeur (optionnel)</param>
        /// <param name="senderEmail">Email de l'expéditeur (optionnel)</param>
        /// <param name="bcc">Copie cachée (optionnel)</param>
        /// <param name="wrapWithTemplate">Encapsuler avec le template header/footer</param>
        /// <param name="language">Langue du courriel (fr ou en)</param>
        /// <returns>True si le courriel a été envoyé avec succès</returns>
        Task<bool> SendEmailAsync(
            string to,
            string subject,
            string body,
            string recipientFirstName,
            string senderName = null,
            string senderEmail = null,
            string bcc = null,
            bool wrapWithTemplate = true,
            string language = "fr");

        /// <summary>
        /// Génère le HTML complet avec header et footer
        /// </summary>
        /// <param name="content">Contenu HTML du corps du message</param>
        /// <param name="recipientFirstName">Prénom du destinataire pour la salutation</param>
        /// <param name="language">Langue du template (fr ou en)</param>
        /// <returns>HTML complet avec template</returns>
        string WrapWithTemplate(string content, string recipientFirstName, string language = "fr");
    }
}
