using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using was.api.Models;
using was.api.Models.Auth;

namespace was.api.Services.Auth
{
    public class AuthService(ILogger<AuthService> logger, AppDbContext dbContext, IOptions<Settings> options) : IAuthService
    {
        private AppDbContext _db = dbContext;
        private ILogger<AuthService> _logger= logger;
        private readonly Settings _settings = options.Value;
        private static readonly object _dummy = new();

        public string GetPasswordHash(string password) {
            var passwordHasher = new PasswordHasher<object>();
            return passwordHasher.HashPassword(_dummy, password);
        }
        public bool VerifyPassword(string passwordHash, string password)
        {
            var passwordHasher = new PasswordHasher<object>();
            var result = passwordHasher.VerifyHashedPassword(_dummy, passwordHash, password);
            return result == PasswordVerificationResult.Success;
        }
        public (string, string) GenerateToken(User user)
        {
            var now = DateTime.UtcNow;
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(_settings.Jwt.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity([
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.SerialNumber, user.Id.ToString()),
                    new Claim(ClaimTypes.Surname, user.LastName),
                    new Claim(ClaimTypes.Name, user.FirstName),
                    new Claim(ClaimTypes.Role, user.RoleName) // For role-based auth
                ]),
                Expires = now.AddHours(12),
                Issuer = _settings.Jwt.Issuer,
                Audience = _settings.Jwt.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var atoken = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(atoken);

            tokenDescriptor.Expires = now.AddDays(7);
            var rtoken = tokenHandler.CreateToken(tokenDescriptor);
            var refreshToken = tokenHandler.WriteToken(rtoken);
            return (accessToken, refreshToken);
        }

        public async Task<bool> SaveRefreshToken(int id, string refresToken)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
            if(user is not null)
            {
                user.RefreshToken = refresToken;
                int rowsAff= await _db.SaveChangesAsync();
                return rowsAff > 0;
            }
            return false;
        }

    }
}
