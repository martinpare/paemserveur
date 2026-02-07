using System;
using System.ComponentModel.DataAnnotations;

namespace serveur.Models.Dtos
{
    /// <summary>
    /// DTO pour la connexion
    /// </summary>
    public class LoginDto
    {
        [Required(ErrorMessage = "Le nom d'utilisateur est requis")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Le mot de passe est requis")]
        public string Password { get; set; }
    }

    /// <summary>
    /// DTO pour l'inscription
    /// </summary>
    public class RegisterDto
    {
        [Required(ErrorMessage = "Le nom d'utilisateur est requis")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Le nom d'utilisateur doit contenir entre 3 et 50 caracteres")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Le mot de passe est requis")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caracteres")]
        public string Password { get; set; }

        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        public string Mail { get; set; }

        [Required(ErrorMessage = "Le prenom est requis")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Le nom est requis")]
        public string LastName { get; set; }
    }

    /// <summary>
    /// DTO pour la reponse de token
    /// </summary>
    public class TokenResponseDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string TokenType { get; set; } = "Bearer";
        public UserInfoDto User { get; set; }
    }

    /// <summary>
    /// DTO pour les informations utilisateur dans la reponse token
    /// Contient toutes les informations sauf le mot de passe et tokens de reset
    /// </summary>
    public class UserInfoDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Mail { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? GenderId { get; set; }
        public bool DarkMode { get; set; }
        public string Avatar { get; set; }
        public string AccentColor { get; set; }
        public string Language { get; set; }
        public int? OrganisationId { get; set; }
        public int? PedagogicalStructureId { get; set; }
        public int? LearningCenterId { get; set; }
        public int? TitleId { get; set; }
        public int? ActiveRoleId { get; set; }
        public string[] Roles { get; set; }
    }

    /// <summary>
    /// DTO pour le renouvellement du token
    /// </summary>
    public class RefreshTokenDto
    {
        [Required(ErrorMessage = "Le refresh token est requis")]
        public string RefreshToken { get; set; }
    }

    /// <summary>
    /// DTO pour le changement de mot de passe
    /// </summary>
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Le mot de passe actuel est requis")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Le nouveau mot de passe est requis")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caracteres")]
        public string NewPassword { get; set; }
    }

    /// <summary>
    /// DTO pour la connexion en mode développement (sans mot de passe)
    /// </summary>
    public class DevLoginDto
    {
        [Required(ErrorMessage = "Le nom d'utilisateur est requis")]
        public string Username { get; set; }
    }

    /// <summary>
    /// DTO pour la connexion avec tenant (organisation)
    /// </summary>
    public class TenantLoginDto
    {
        [Required(ErrorMessage = "Le nom d'utilisateur est requis")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Le mot de passe est requis")]
        public string Password { get; set; }

        [Required(ErrorMessage = "L'organisation est requise")]
        public int OrganisationId { get; set; }
    }

    /// <summary>
    /// DTO pour la liste des utilisateurs disponibles au login (endpoint public)
    /// Contient uniquement les informations non sensibles nécessaires à l'affichage
    /// </summary>
    public class LoginUserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
    }

    /// <summary>
    /// DTO pour la connexion d'un apprenant
    /// </summary>
    public class LearnerLoginDto
    {
        [Required(ErrorMessage = "Le code permanent est requis")]
        public string PermanentCode { get; set; }

        [Required(ErrorMessage = "Le mot de passe est requis")]
        public string Password { get; set; }
    }

    /// <summary>
    /// DTO pour la connexion d'un apprenant en mode développement (sans mot de passe)
    /// </summary>
    public class DevLearnerLoginDto
    {
        [Required(ErrorMessage = "Le code permanent est requis")]
        public string PermanentCode { get; set; }
    }

    /// <summary>
    /// DTO pour les informations d'un apprenant authentifié
    /// </summary>
    public class LearnerInfoDto
    {
        public int Id { get; set; }
        public string PermanentCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public int LearningCenterId { get; set; }
        public int? GroupId { get; set; }
        public int LanguageId { get; set; }
        public bool HasAccommodations { get; set; }
        public string Avatar { get; set; }
    }

    /// <summary>
    /// DTO pour la réponse de token d'un apprenant
    /// </summary>
    public class LearnerTokenResponseDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string TokenType { get; set; } = "Bearer";
        public LearnerInfoDto Learner { get; set; }
    }

    /// <summary>
    /// DTO pour la demande de réinitialisation de mot de passe
    /// </summary>
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        public string Email { get; set; }
    }

    /// <summary>
    /// DTO pour la réponse de demande de réinitialisation de mot de passe
    /// </summary>
    public class ForgotPasswordResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public bool UserExists { get; set; }
        public int? UserId { get; set; }
        public string ResetToken { get; set; }
    }

    /// <summary>
    /// DTO pour la validation d'un token de réinitialisation
    /// </summary>
    public class ValidateResetTokenDto
    {
        [Required(ErrorMessage = "Le token est requis")]
        public string Token { get; set; }
    }

    /// <summary>
    /// DTO pour la réponse de validation d'un token de réinitialisation
    /// </summary>
    public class ValidateResetTokenResponseDto
    {
        public bool Valid { get; set; }
        public string Message { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
    }

    /// <summary>
    /// DTO pour la réinitialisation du mot de passe
    /// </summary>
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "Le token est requis")]
        public string Token { get; set; }

        [Required(ErrorMessage = "Le nouveau mot de passe est requis")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères")]
        public string NewPassword { get; set; }
    }

    /// <summary>
    /// DTO pour la réponse de réinitialisation du mot de passe
    /// </summary>
    public class ResetPasswordResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
