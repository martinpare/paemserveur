using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using serveur.Data;
using serveur.Models.Dtos;
using serveur.Models.Entities;
using serveur.Services;

namespace serveur.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            AppDbContext context,
            IJwtService jwtService,
            IConfiguration configuration,
            ILogger<AuthController> logger)
        {
            _context = context;
            _jwtService = jwtService;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir la liste des utilisateurs pour la page de login (endpoint public)
        /// Retourne uniquement les informations non sensibles
        /// </summary>
        [HttpGet("login-users")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<LoginUserDto>>> GetLoginUsers()
        {
            try
            {
                var users = await _context.Users
                    .Select(u => new LoginUserDto
                    {
                        Id = u.Id,
                        Username = u.Username,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Avatar = u.Avatar
                    })
                    .OrderBy(u => u.LastName)
                    .ThenBy(u => u.FirstName)
                    .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des utilisateurs de login");
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        /// <summary>
        /// Connexion d'un utilisateur
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

                if (user == null)
                {
                    return Unauthorized(new { message = "Nom d'utilisateur ou mot de passe incorrect", code = "INVALID_CREDENTIALS" });
                }

                if (!VerifyPassword(loginDto.Password, user.Password))
                {
                    return Unauthorized(new { message = "Nom d'utilisateur ou mot de passe incorrect", code = "INVALID_CREDENTIALS" });
                }

                if (!user.Active)
                {
                    return Unauthorized(new { message = "Votre compte utilisateur est désactivé. Veuillez contacter votre administrateur.", code = "ACCOUNT_DISABLED" });
                }

                var roles = await GetUserRoles(user.Id);
                var accessToken = _jwtService.GenerateAccessToken(user, roles);
                var refreshToken = _jwtService.GenerateRefreshToken();

                var refreshTokenExpirationDays = int.Parse(_configuration["Jwt:RefreshTokenExpirationDays"] ?? "7");

                var refreshTokenEntity = new RefreshToken
                {
                    Token = refreshToken,
                    UserId = user.Id,
                    ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpirationDays),
                    CreatedAt = DateTime.UtcNow
                };

                _context.RefreshTokens.Add(refreshTokenEntity);
                await _context.SaveChangesAsync();

                var accessTokenExpirationMinutes = int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "60");

                return Ok(new TokenResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(accessTokenExpirationMinutes),
                    User = MapToUserInfoDto(user, roles)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la connexion");
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        /// <summary>
        /// Inscription d'un nouvel utilisateur
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<TokenResponseDto>> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
                {
                    return BadRequest(new { message = "Ce nom d'utilisateur est deja utilise" });
                }

                if (await _context.Users.AnyAsync(u => u.Mail == registerDto.Mail))
                {
                    return BadRequest(new { message = "Cette adresse email est deja utilisee" });
                }

                var user = new User
                {
                    Username = registerDto.Username,
                    Password = HashPassword(registerDto.Password),
                    Mail = registerDto.Mail,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var roles = new List<string>();
                var accessToken = _jwtService.GenerateAccessToken(user, roles);
                var refreshToken = _jwtService.GenerateRefreshToken();

                var refreshTokenExpirationDays = int.Parse(_configuration["Jwt:RefreshTokenExpirationDays"] ?? "7");

                var refreshTokenEntity = new RefreshToken
                {
                    Token = refreshToken,
                    UserId = user.Id,
                    ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpirationDays),
                    CreatedAt = DateTime.UtcNow
                };

                _context.RefreshTokens.Add(refreshTokenEntity);
                await _context.SaveChangesAsync();

                var accessTokenExpirationMinutes = int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "60");

                return CreatedAtAction(nameof(GetCurrentUser), new TokenResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(accessTokenExpirationMinutes),
                    User = MapToUserInfoDto(user, roles)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'inscription");
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        /// <summary>
        /// Renouvellement du token d'acces
        /// </summary>
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            try
            {
                var storedToken = await _context.RefreshTokens
                    .Include(rt => rt.User)
                    .FirstOrDefaultAsync(rt => rt.Token == refreshTokenDto.RefreshToken);

                if (storedToken == null)
                {
                    return Unauthorized(new { message = "Refresh token invalide" });
                }

                if (!storedToken.IsActive)
                {
                    return Unauthorized(new { message = "Refresh token expire ou revoque", code = "TOKEN_EXPIRED" });
                }

                // Vérifier si l'utilisateur est toujours actif
                if (!storedToken.User.Active)
                {
                    // Révoquer le token si l'utilisateur est désactivé
                    storedToken.IsRevoked = true;
                    storedToken.RevokedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    return Unauthorized(new { message = "Votre compte utilisateur est désactivé. Veuillez contacter votre administrateur.", code = "ACCOUNT_DISABLED" });
                }

                // Revoquer l'ancien token
                storedToken.IsRevoked = true;
                storedToken.RevokedAt = DateTime.UtcNow;

                var user = storedToken.User;
                var roles = await GetUserRoles(user.Id);
                var newAccessToken = _jwtService.GenerateAccessToken(user, roles);
                var newRefreshToken = _jwtService.GenerateRefreshToken();

                storedToken.ReplacedByToken = newRefreshToken;

                var refreshTokenExpirationDays = int.Parse(_configuration["Jwt:RefreshTokenExpirationDays"] ?? "7");

                var newRefreshTokenEntity = new RefreshToken
                {
                    Token = newRefreshToken,
                    UserId = user.Id,
                    ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpirationDays),
                    CreatedAt = DateTime.UtcNow
                };

                _context.RefreshTokens.Add(newRefreshTokenEntity);
                await _context.SaveChangesAsync();

                var accessTokenExpirationMinutes = int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "60");

                return Ok(new TokenResponseDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(accessTokenExpirationMinutes),
                    User = MapToUserInfoDto(user, roles)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du renouvellement du token");
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        /// <summary>
        /// Deconnexion (revocation du refresh token)
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenDto refreshTokenDto)
        {
            try
            {
                var storedToken = await _context.RefreshTokens
                    .FirstOrDefaultAsync(rt => rt.Token == refreshTokenDto.RefreshToken);

                if (storedToken != null && storedToken.IsActive)
                {
                    storedToken.IsRevoked = true;
                    storedToken.RevokedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }

                return Ok(new { message = "Deconnexion reussie" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la deconnexion");
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        /// <summary>
        /// Obtenir les informations de l'utilisateur connecte
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserInfoDto>> GetCurrentUser()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new { message = "Token invalide" });
                }

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "Utilisateur non trouve" });
                }

                var roles = await GetUserRoles(userId);

                return Ok(MapToUserInfoDto(user, roles));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la recuperation de l'utilisateur courant");
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        /// <summary>
        /// Connexion en mode développement (sans mot de passe)
        /// ATTENTION: Ne fonctionne qu'en environnement Development
        /// </summary>
        [HttpPost("dev-login")]
        [AllowAnonymous]
        public async Task<ActionResult<TokenResponseDto>> DevLogin(
            [FromBody] DevLoginDto devLoginDto,
            [FromServices] IWebHostEnvironment env)
        {
            // Vérifier qu'on est bien en mode développement
            if (!env.IsDevelopment())
            {
                return NotFound(); // Retourne 404 en production pour masquer l'endpoint
            }

            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == devLoginDto.Username);

                if (user == null)
                {
                    return Unauthorized(new { message = "Utilisateur non trouvé", code = "USER_NOT_FOUND" });
                }

                if (!user.Active)
                {
                    return Unauthorized(new { message = "Votre compte utilisateur est désactivé. Veuillez contacter votre administrateur.", code = "ACCOUNT_DISABLED" });
                }

                _logger.LogWarning("DEV LOGIN: Connexion sans mot de passe pour {Username}", user.Username);

                var roles = await GetUserRoles(user.Id);
                var accessToken = _jwtService.GenerateAccessToken(user, roles);
                var refreshToken = _jwtService.GenerateRefreshToken();

                var refreshTokenExpirationDays = int.Parse(_configuration["Jwt:RefreshTokenExpirationDays"] ?? "7");

                var refreshTokenEntity = new RefreshToken
                {
                    Token = refreshToken,
                    UserId = user.Id,
                    ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpirationDays),
                    CreatedAt = DateTime.UtcNow
                };

                _context.RefreshTokens.Add(refreshTokenEntity);
                await _context.SaveChangesAsync();

                var accessTokenExpirationMinutes = int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "60");

                return Ok(new TokenResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(accessTokenExpirationMinutes),
                    User = MapToUserInfoDto(user, roles)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la connexion dev");
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        /// <summary>
        /// Connexion d'un utilisateur avec validation du tenant (organisation)
        /// </summary>
        [HttpPost("tenant-login")]
        [AllowAnonymous]
        public async Task<ActionResult<TokenResponseDto>> TenantLogin([FromBody] TenantLoginDto loginDto)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == loginDto.Username && u.OrganisationId == loginDto.OrganisationId);

                if (user == null)
                {
                    return Unauthorized(new { message = "Nom d'utilisateur ou mot de passe incorrect, ou utilisateur non associé à cette organisation", code = "INVALID_CREDENTIALS" });
                }

                if (!VerifyPassword(loginDto.Password, user.Password))
                {
                    return Unauthorized(new { message = "Nom d'utilisateur ou mot de passe incorrect", code = "INVALID_CREDENTIALS" });
                }

                if (!user.Active)
                {
                    return Unauthorized(new { message = "Votre compte utilisateur est désactivé. Veuillez contacter votre administrateur.", code = "ACCOUNT_DISABLED" });
                }

                var roles = await GetUserRoles(user.Id);
                var accessToken = _jwtService.GenerateAccessToken(user, roles);
                var refreshToken = _jwtService.GenerateRefreshToken();

                var refreshTokenExpirationDays = int.Parse(_configuration["Jwt:RefreshTokenExpirationDays"] ?? "7");

                var refreshTokenEntity = new RefreshToken
                {
                    Token = refreshToken,
                    UserId = user.Id,
                    ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpirationDays),
                    CreatedAt = DateTime.UtcNow
                };

                _context.RefreshTokens.Add(refreshTokenEntity);
                await _context.SaveChangesAsync();

                var accessTokenExpirationMinutes = int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "60");

                return Ok(new TokenResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(accessTokenExpirationMinutes),
                    User = MapToUserInfoDto(user, roles)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la connexion avec tenant");
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        /// <summary>
        /// Connexion d'un apprenant via code permanent et mot de passe
        /// </summary>
        [HttpPost("learner-login")]
        [AllowAnonymous]
        public async Task<ActionResult<LearnerTokenResponseDto>> LearnerLogin([FromBody] LearnerLoginDto loginDto)
        {
            try
            {
                var learner = await _context.Learners
                    .FirstOrDefaultAsync(l => l.PermanentCode == loginDto.PermanentCode);

                if (learner == null)
                {
                    return Unauthorized(new { message = "Code permanent ou mot de passe incorrect", code = "INVALID_CREDENTIALS" });
                }

                if (!VerifyPassword(loginDto.Password, learner.NativePassword))
                {
                    return Unauthorized(new { message = "Code permanent ou mot de passe incorrect", code = "INVALID_CREDENTIALS" });
                }

                if (!learner.IsActive)
                {
                    return Unauthorized(new { message = "Votre compte apprenant est désactivé. Veuillez contacter votre centre d'apprentissage.", code = "ACCOUNT_DISABLED" });
                }

                _logger.LogInformation("Connexion réussie pour l'apprenant {PermanentCode}", learner.PermanentCode);

                var accessToken = _jwtService.GenerateLearnerAccessToken(learner);
                var accessTokenExpirationMinutes = int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "60");

                return Ok(new LearnerTokenResponseDto
                {
                    AccessToken = accessToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(accessTokenExpirationMinutes),
                    Learner = MapToLearnerInfoDto(learner)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la connexion apprenant");
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        /// <summary>
        /// Connexion d'un apprenant en mode développement (sans mot de passe)
        /// ATTENTION: Ne fonctionne qu'en environnement Development
        /// </summary>
        [HttpPost("dev-learner-login")]
        [AllowAnonymous]
        public async Task<ActionResult<LearnerTokenResponseDto>> DevLearnerLogin(
            [FromBody] DevLearnerLoginDto devLoginDto,
            [FromServices] IWebHostEnvironment env)
        {
            // Vérifier qu'on est bien en mode développement
            if (!env.IsDevelopment())
            {
                return NotFound(); // Retourne 404 en production pour masquer l'endpoint
            }

            try
            {
                var learner = await _context.Learners
                    .FirstOrDefaultAsync(l => l.PermanentCode == devLoginDto.PermanentCode);

                if (learner == null)
                {
                    return Unauthorized(new { message = "Apprenant non trouvé", code = "LEARNER_NOT_FOUND" });
                }

                if (!learner.IsActive)
                {
                    return Unauthorized(new { message = "Votre compte apprenant est désactivé. Veuillez contacter votre centre d'apprentissage.", code = "ACCOUNT_DISABLED" });
                }

                _logger.LogWarning("DEV LEARNER LOGIN: Connexion sans mot de passe pour {PermanentCode}", learner.PermanentCode);

                var accessToken = _jwtService.GenerateLearnerAccessToken(learner);
                var accessTokenExpirationMinutes = int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "60");

                return Ok(new LearnerTokenResponseDto
                {
                    AccessToken = accessToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(accessTokenExpirationMinutes),
                    Learner = MapToLearnerInfoDto(learner)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la connexion dev apprenant");
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        /// <summary>
        /// Demande de réinitialisation de mot de passe (endpoint public)
        /// Retourne les informations nécessaires pour envoyer l'email de réinitialisation
        /// </summary>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<ActionResult<ForgotPasswordResponseDto>> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Mail == forgotPasswordDto.Email);

                if (user == null)
                {
                    // Retourner une réponse indiquant que l'utilisateur n'existe pas
                    return Ok(new ForgotPasswordResponseDto
                    {
                        Success = false,
                        Message = "Aucun compte n'est associé à cette adresse email",
                        UserExists = false
                    });
                }

                // Générer un token de réinitialisation
                var resetToken = Guid.NewGuid().ToString("N");
                var resetTokenExpiry = DateTime.UtcNow.AddHours(24);

                // Sauvegarder le token dans la base de données
                user.ResetToken = resetToken;
                user.ResetTokenExpiry = resetTokenExpiry;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Token de réinitialisation créé pour l'utilisateur {UserId}", user.Id);

                return Ok(new ForgotPasswordResponseDto
                {
                    Success = true,
                    Message = "Token de réinitialisation créé avec succès",
                    UserExists = true,
                    UserId = user.Id,
                    ResetToken = resetToken
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la demande de réinitialisation de mot de passe");
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        /// <summary>
        /// Valide un token de réinitialisation de mot de passe (endpoint public)
        /// </summary>
        [HttpPost("validate-reset-token")]
        [AllowAnonymous]
        public async Task<ActionResult<ValidateResetTokenResponseDto>> ValidateResetToken([FromBody] ValidateResetTokenDto validateDto)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.ResetToken == validateDto.Token);

                if (user == null)
                {
                    return Ok(new ValidateResetTokenResponseDto
                    {
                        Valid = false,
                        Message = "Token invalide ou inexistant"
                    });
                }

                // Vérifier si le token n'est pas expiré
                if (user.ResetTokenExpiry.HasValue && user.ResetTokenExpiry.Value < DateTime.UtcNow)
                {
                    return Ok(new ValidateResetTokenResponseDto
                    {
                        Valid = false,
                        Message = "Le token a expiré"
                    });
                }

                return Ok(new ValidateResetTokenResponseDto
                {
                    Valid = true,
                    Message = "Token valide",
                    FirstName = user.FirstName,
                    Email = user.Mail
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la validation du token de réinitialisation");
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        /// <summary>
        /// Réinitialise le mot de passe avec un token (endpoint public)
        /// </summary>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<ActionResult<ResetPasswordResponseDto>> ResetPassword([FromBody] ResetPasswordDto resetDto)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.ResetToken == resetDto.Token);

                if (user == null)
                {
                    return Ok(new ResetPasswordResponseDto
                    {
                        Success = false,
                        Message = "Token invalide ou inexistant"
                    });
                }

                // Vérifier si le token n'est pas expiré
                if (user.ResetTokenExpiry.HasValue && user.ResetTokenExpiry.Value < DateTime.UtcNow)
                {
                    return Ok(new ResetPasswordResponseDto
                    {
                        Success = false,
                        Message = "Le token a expiré"
                    });
                }

                // Mettre à jour le mot de passe
                user.Password = HashPassword(resetDto.NewPassword);
                user.ResetToken = null;
                user.ResetTokenExpiry = null;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Mot de passe réinitialisé pour l'utilisateur {UserId}", user.Id);

                return Ok(new ResetPasswordResponseDto
                {
                    Success = true,
                    Message = "Mot de passe réinitialisé avec succès"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la réinitialisation du mot de passe");
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        /// <summary>
        /// Changement de mot de passe
        /// </summary>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new { message = "Token invalide" });
                }

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "Utilisateur non trouve" });
                }

                if (!VerifyPassword(changePasswordDto.CurrentPassword, user.Password))
                {
                    return BadRequest(new { message = "Mot de passe actuel incorrect" });
                }

                user.Password = HashPassword(changePasswordDto.NewPassword);
                await _context.SaveChangesAsync();

                // Revoquer tous les refresh tokens de l'utilisateur
                var userTokens = await _context.RefreshTokens
                    .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                    .ToListAsync();

                foreach (var token in userTokens)
                {
                    token.IsRevoked = true;
                    token.RevokedAt = DateTime.UtcNow;
                }
                await _context.SaveChangesAsync();

                return Ok(new { message = "Mot de passe modifie avec succes" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du changement de mot de passe");
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        private async Task<IList<string>> GetUserRoles(int userId)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Join(_context.Roles,
                    ur => ur.RoleId,
                    r => r.Id,
                    (ur, r) => r.Code)
                .ToListAsync();
        }

        private UserInfoDto MapToUserInfoDto(User user, IList<string> roles)
        {
            return new UserInfoDto
            {
                Id = user.Id,
                Username = user.Username,
                Mail = user.Mail,
                FirstName = user.FirstName,
                LastName = user.LastName,
                GenderId = user.GenderId,
                DarkMode = user.DarkMode,
                Avatar = user.Avatar,
                AccentColor = user.AccentColor,
                Language = user.Language,
                OrganisationId = user.OrganisationId,
                PedagogicalStructureId = user.PedagogicalStructureId,
                LearningCenterId = user.LearningCenterId,
                TitleId = user.TitleId,
                ActiveRoleId = user.ActiveRoleId,
                Roles = roles.ToArray()
            };
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(hashedPassword))
            {
                return false;
            }

            // Support des mots de passe BCrypt (commencent par $2)
            if (hashedPassword.StartsWith("$2"))
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            // Support des anciens mots de passe SHA256 (Base64, environ 44 caractères)
            else if (hashedPassword.Length >= 40)
            {
                using (var sha256 = System.Security.Cryptography.SHA256.Create())
                {
                    var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                    var sha256Hash = Convert.ToBase64String(hashedBytes);
                    return sha256Hash == hashedPassword;
                }
            }
            // Support temporaire des mots de passe en clair (à retirer après migration complète)
            else
            {
                return password == hashedPassword;
            }
        }

        private LearnerInfoDto MapToLearnerInfoDto(Learner learner)
        {
            return new LearnerInfoDto
            {
                Id = learner.Id,
                PermanentCode = learner.PermanentCode,
                FirstName = learner.FirstName,
                LastName = learner.LastName,
                DateOfBirth = learner.DateOfBirth,
                Email = learner.Email,
                LearningCenterId = learner.LearningCenterId,
                GroupId = learner.GroupId,
                LanguageId = learner.LanguageId,
                HasAccommodations = learner.HasAccommodations,
                Avatar = learner.Avatar
            };
        }
    }
}
