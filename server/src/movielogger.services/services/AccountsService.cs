using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using movielogger.dal;
using movielogger.services.interfaces;
using movielogger.dal.entities;
using BC = BCrypt.Net.BCrypt;

namespace movielogger.services.services;

public class AccountsService : IAccountsService
{
    private readonly IAssessmentDbContext _db;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public AccountsService(IAssessmentDbContext db, IMapper mapper, IConfiguration configuration)
    {
        _db = db;
        _mapper = mapper;
        _configuration = configuration;
    }
    
    public async Task<(string token, User user)> AuthenticateUserAsync(string email, string password)
    {
        try
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == email && !x.IsDeleted);
            if (user == null) throw new UnauthorizedAccessException("Invalid email or password");

            if (!BC.Verify(password, user.Password))
                throw new UnauthorizedAccessException("Invalid email or password");

            var token = GenerateJwtToken(user);
            return (token, user);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Authentication error: {ex}");
            throw;
        }
    }

    private string GenerateJwtToken(User user)
    {
        try
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key not configured"));
            var issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured");
            var audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("JWT Audience not configured");
            var expiresInMinutes = int.Parse(jwtSettings["ExpiresInMinutes"] ?? "60");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
                }),
                Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Token generation error: {ex}");
            throw;
        }
    }
}