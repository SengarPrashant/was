using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using was.api.Models;
using was.api.Models.Auth;
using was.api.Services.Auth;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace was.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(ILogger<AuthController> logger, IOptions<Settings> options, IUserManagementService userManagementService) : ControllerBase
    {
        private readonly ILogger<AuthController> _logger = logger;
        private readonly Settings _settings = options.Value;
        private readonly IUserManagementService _userService = userManagementService;

        //// GET: api/<AuthController>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/<AuthController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/<AuthController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] LoginRequest request)
        {
            // var u = await _db.Roles.ToListAsync();
            // return Ok(u);

            try
            {
                _logger.LogInformation($"Received login request for user: {request.UserName}");

                var result = await _userService.AuthenticateUser(request);

                if (result == null) {
                    return Ok("Inavlid username/password.");
                }
                return Ok(result);
                
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while processing login for user: {request.UserName}", ex);
                return StatusCode(500, "Something went wrong on the server.");
            }
           
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                // Get the user by the refresh token
                //var user = await _userManager.Users
                //    .FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken && u.RefreshTokenExpiration > DateTime.UtcNow);
                var user = new User { Email = "pss@gmail.com", FirstName = "Prashant", LastName = "Singh", RoleName = "Admin" };
                if (user == null)
                {
                    return Unauthorized("Invalid refresh token.");
                }

                // Generate new JWT and refresh token
                var (accessToken, refreshToken) = GenerateToken(user);

                return Ok(new { accessToken, refreshToken });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while processing RefreshToken: {request.UserName}", ex);
                return StatusCode(500, "Something went wrong on the server.");
            }
            
        }

        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                // logic
                var user = new User { Email = "pss@gmail.com", FirstName = "Prashant", LastName = "Singh", RoleName = "Admin" };
                return Ok("Success");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while processing ChangePassword: {request.UserName}", ex);
                return StatusCode(500, "Something went wrong on the server.");
            }
        }

        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                // logic
                var user = new User { Email = "pss@gmail.com", FirstName = "Prashant", LastName = "Singh", RoleName = "Admin" };
                return Ok("Success");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while processing ResetPassword: {request.UserName}", ex);
                return StatusCode(500, "Something went wrong on the server.");
            }
        }

        //// PUT api/<AuthController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<AuthController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}

        private (string, string) GenerateToken(User user)
        {
            var now = DateTime.UtcNow;
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(_settings.Jwt.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Name, "test user"),
                new Claim(ClaimTypes.Role, "admin") // For role-based auth
                }),
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

    }
}
