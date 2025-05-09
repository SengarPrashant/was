using was.api.Models;
using was.api.Models.Auth;

namespace was.api.Services.Auth
{
    public class UserManagementService(ILogger<UserManagementService> logger, AppDbContext dbContext, IAuthService authService) : IUserManagementService
    {
        private AppDbContext _db = dbContext;
        private ILogger<UserManagementService> _logger = logger;
        private IAuthService _auth= authService;

        public async Task<NewUserRequest> CreateNewUser(NewUserRequest user)
        {
            try
            {
                user.Password = _auth.GetPasswordHash(user.Password);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while creating new user: {user.FirstName} {user.LastName}");
                throw;
            }
        }
        public async Task<LoginResponse?> AuthenticateUser(LoginRequest request)
        {
            try
            {
                var res = new LoginResponse();
                // get user from db
                var user = new User();
                user.Password = "test";

                var hassgedPassword = _auth.GetPasswordHash(request.Password);
                if(_auth.VerifyPassword(hassgedPassword, user.Password))
                {
                    var (token, refreshToken) = _auth.GenerateToken(user);

                    if(await _auth.SaveRefreshToken(user.Id, refreshToken))
                    {
                        res.AccessToken = token;
                        res.RefreshToken = refreshToken;

                        user.Password = null;
                        user.PasswordOtp = null;
                        user.RefreshToken = null;

                        return res;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while authenticating user: {request.UserName}");
                throw;
            }
        }
    }
}
