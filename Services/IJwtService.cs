using System.Collections.Generic;
using System.Security.Claims;
using serveur.Models.Entities;

namespace serveur.Services
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user, IList<string> roles);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        int? GetUserIdFromToken(string token);
    }
}
