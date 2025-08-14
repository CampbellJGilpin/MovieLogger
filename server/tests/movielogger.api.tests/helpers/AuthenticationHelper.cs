using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using movielogger.dal.entities;

namespace movielogger.api.tests.helpers;

public static class AuthenticationHelper
{
    private static readonly string TestJwtKey = "8a6b89da22b854f58f57f15bb675d2692255801f989b24098d6266fc1f4857f13fdace450ac0503fd68db511a8520ff106a16ce31f1402a6ae4ffc3e1849b531";
    private static readonly string TestIssuer = "MovieLoggerAPI";
    private static readonly string TestAudience = "MovieLoggerUsers";

    public static string GenerateJwtToken(User user)
    {
        var key = Encoding.UTF8.GetBytes(TestJwtKey);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = TestIssuer,
            Audience = TestAudience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public static void AddAuthorizationHeader(HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }
}