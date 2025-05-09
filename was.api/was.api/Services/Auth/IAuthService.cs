using Microsoft.AspNetCore.Identity;
using was.api.Models.Auth;

namespace was.api.Services.Auth
{
    public interface IAuthService
    {
        public string GetPasswordHash(string password);
        public bool VerifyPassword(string passwordHash, string password);
        public (string, string) GenerateToken(User user);
        public Task<bool> SaveRefreshToken(int id, string refresToken);
    }
}
