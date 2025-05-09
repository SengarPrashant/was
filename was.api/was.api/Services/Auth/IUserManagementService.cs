using was.api.Models.Auth;

namespace was.api.Services.Auth
{
    public interface IUserManagementService
    {
        public Task<LoginResponse?> AuthenticateUser(LoginRequest request);
        public Task<NewUserRequest> CreateNewUser(NewUserRequest user);
    }
}
