using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using was.api.Models;
using was.api.Models.Auth;
using was.api.Services.Auth;

namespace was.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(ILogger<UserController> logger, IOptions<Settings> options, IUserManagementService userManagementService) : ControllerBase
    {
        private readonly ILogger<UserController> _logger = logger;
        private readonly Settings _settings = options.Value;
        private readonly IUserManagementService _userService = userManagementService;

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] User request)
        {
            // var u = await _db.Roles.ToListAsync();
            // return Ok(u);

            try
            {
                _logger.LogInformation($"Received login request for user: {request.Email}");

                var result = await _userService.CreateUser(request);

                if (result == null)
                {
                    return Ok("Inavlid username/password.");
                }
                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while creating user user: {request.Email}", ex);
                return StatusCode(500, "Something went wrong on the server.");
            }

        }
    }
}
