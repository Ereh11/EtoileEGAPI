using Domain.Entities;
using System.Security.Claims;

namespace Domain.Interfaces.JwtTokenGenerator;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(ApplicationUser user, IList<string> roles);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
