using System.ComponentModel.DataAnnotations;

namespace serveur.Models.Dtos
{
    /// <summary>
    /// DTO pour l'envoi d'un courriel
    /// </summary>
    public class SendMailDto
    {
        [Required(ErrorMessage = "L'adresse du destinataire est requise")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        public string Mail { get; set; }

        [Required(ErrorMessage = "Le sujet est requis")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Le corps du message est requis")]
        public string Body { get; set; }

        /// <summary>
        /// Nom de l'expéditeur (optionnel, utilise la valeur par défaut si non spécifié)
        /// </summary>
        public string SenderName { get; set; }

        /// <summary>
        /// Email de l'expéditeur (optionnel, utilise la valeur par défaut si non spécifié)
        /// </summary>
        public string SenderMail { get; set; }

        /// <summary>
        /// Adresse en copie cachée (optionnel)
        /// </summary>
        public string BccMail { get; set; }

        /// <summary>
        /// Si true, encapsule le body avec le template header/footer
        /// Par défaut: true
        /// </summary>
        public bool WrapWithTemplate { get; set; } = true;

        /// <summary>
        /// Langue du courriel (fr ou en)
        /// Par défaut: fr
        /// </summary>
        public string Language { get; set; } = "fr";

        /// <summary>
        /// Prénom du destinataire pour la salutation "Bonjour [Prénom],"
        /// Si non fourni, sera récupéré automatiquement à partir de l'adresse courriel
        /// </summary>
        public string RecipientFirstName { get; set; }
    }

    /// <summary>
    /// DTO pour la réponse d'envoi de courriel
    /// </summary>
    public class SendMailResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
