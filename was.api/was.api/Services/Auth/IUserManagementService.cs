using was.api.Models.Auth;

namespace was.api.Services.Auth
{
    public interface IUserManagementService
    {
        public Task<LoginResponse?> AuthenticateUser(LoginRequest request);
        public Task<bool> ChangePassword(ChangePasswordRequest request);
        public Task<User> CreateUser(User user);
    }
}
