using Microsoft.EntityFrameworkCore;
using was.api.Models;
using was.api.Models.Auth;

namespace was.api.Services.Auth
{
    public class UserManagementService(ILogger<UserManagementService> logger, AppDbContext dbContext, IAuthService authService) : IUserManagementService
    {
        private AppDbContext _db = dbContext;
        private ILogger<UserManagementService> _logger = logger;
        private IAuthService _auth= authService;

        public async Task<bool> ChangePassword(ChangePasswordRequest request)
        {
            try
            {
                var user = await _db.Users.Where(x => x.Id == request.Id).FirstOrDefaultAsync();

                if(user == null) return false;
                if (_auth.VerifyPassword(user.Password, request.OldPassword.Trim()))
                {
                    user.Password = _auth.GetPasswordHash(request.NewPassword.Trim());
                    int rowsAff = await _db.SaveChangesAsync();
                    return rowsAff > 0;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while changing password for: {request.Email}");
                throw;
            }
        }
        public async Task<LoginResponse?> AuthenticateUser(LoginRequest request)
        {
            try
            {
                var res = new LoginResponse();
                // get user from db
                var user = await (from u in _db.Users
                                    join r in _db.Roles
                                    on u.RoleId equals r.Id
                                    select new User
                                    {
                                        Id = u.Id,
                                        Email = u.Email,
                                        FirstName = u.FirstName,
                                        LastName = u.LastName,
                                        Password = u.Password,
                                        ActiveStatus = u.ActiveStatus,
                                        RoleId =   u.RoleId,
                                        RoleName =r.Name
                                    }).FirstOrDefaultAsync();
                if (user is null) return null;

               // var hassgedPassword = _auth.GetPasswordHash(request.Password);
                if(_auth.VerifyPassword(user.Password, request.Password.Trim()))
                {
                    var (token, refreshToken) = _auth.GenerateToken(user);

                    if(await _auth.SaveRefreshToken(user.Id, refreshToken))
                    {
                        res.AccessToken = token;
                        res.RefreshToken = refreshToken;
                        user.Password = null;
                        user.PasswordOtp = null;
                        user.RefreshToken = null;
                        res.UserDetails = user;
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
        public async Task<User> CreateUser(User user)
        {
            try
            {
                var newUser = new Models.Dtos.DtoUser {
                    Id = user.Id, Email = user.Email.Trim(), 
                    FirstName = user.FirstName.Trim(), LastName=user.LastName.Trim(),
                    RoleId =user.RoleId,
                    ActiveStatus = 1, // active 
                    Password =_auth.GetPasswordHash(user.Password.Trim()),
                };
                _db.Users.Add(newUser);
                await _db.SaveChangesAsync();
                user.Id = newUser.Id;
                return user;
            }
            catch (Exception e)
            {
                _logger.LogError($"Error while creating user: {user.Email}");
                throw;
            }
        }
    }
}
