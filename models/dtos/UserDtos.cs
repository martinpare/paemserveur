using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace serveur.Models.Dtos
{
    /// <summary>
    /// DTO pour afficher un utilisateur (sans mot de passe ni tokens)
    /// </summary>
    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Sexe { get; set; }
        public string Username { get; set; }
        public string Mail { get; set; }
        public bool DarkMode { get; set; }
        public string Avatar { get; set; }
        public string AccentColor { get; set; }
        public string Language { get; set; }
        public int? OrganisationId { get; set; }
        public int? PedagogicalStructureId { get; set; }
        public int? LearningCenterId { get; set; }
        public int? TitleId { get; set; }
        public int? ActiveRoleId { get; set; }
    }

    /// <summary>
    /// DTO pour créer un utilisateur
    /// </summary>
    public class UserCreateDto
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(1)]
        public string Sexe { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(255)]
        [EmailAddress]
        public string Mail { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 6)]
        public string Password { get; set; }

        public int? OrganisationId { get; set; }
        public int? PedagogicalStructureId { get; set; }
        public int? LearningCenterId { get; set; }
        public int? TitleId { get; set; }
    }

    /// <summary>
    /// DTO pour mettre à jour un utilisateur (sans mot de passe)
    /// </summary>
    public class UserUpdateDto
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(1)]
        public string Sexe { get; set; }

        [Required]
        [StringLength(255)]
        [EmailAddress]
        public string Mail { get; set; }

        public int? OrganisationId { get; set; }
        public int? PedagogicalStructureId { get; set; }
        public int? LearningCenterId { get; set; }
        public int? TitleId { get; set; }
    }

    /// <summary>
    /// DTO pour changer le mot de passe
    /// </summary>
    public class PasswordChangeDto
    {
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 6)]
        public string NewPassword { get; set; }
    }

    /// <summary>
    /// DTO pour mettre à jour les préférences utilisateur
    /// </summary>
    public class UserPreferencesDto
    {
        public bool DarkMode { get; set; }

        [StringLength(20)]
        public string AccentColor { get; set; }

        [StringLength(10)]
        public string Language { get; set; }

        public string Avatar { get; set; }

        public int? ActiveRoleId { get; set; }
    }

    /// <summary>
    /// DTO pour mise à jour partielle (PATCH) - tous les champs sont optionnels
    /// </summary>
    public class UserPatchDto
    {
        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(1)]
        public string Sexe { get; set; }

        [StringLength(255)]
        [EmailAddress]
        public string Mail { get; set; }

        public bool? DarkMode { get; set; }

        public string Avatar { get; set; }

        [StringLength(20)]
        public string AccentColor { get; set; }

        [StringLength(10)]
        public string Language { get; set; }

        public int? OrganisationId { get; set; }
        public int? PedagogicalStructureId { get; set; }
        public int? LearningCenterId { get; set; }
        public int? TitleId { get; set; }
        public int? ActiveRoleId { get; set; }
    }

    /// <summary>
    /// DTO pour synchroniser les rôles d'un utilisateur
    /// </summary>
    public class SyncUserRolesDto
    {
        [Required]
        public int OrganisationId { get; set; }

        [Required]
        public List<int> RoleIds { get; set; }
    }
}
