using Capstone.Common.DTOs.User;
using Capstone.Service.LoggerService;
using Capstone.Service.UserService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Capstone.API.Controllers
{
    [Route("api/user-management")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IUserService _usersService;

        public UserController(ILoggerManager logger, IUserService usersService)
        {
            _logger = logger;
            _usersService = usersService;
        }

        [HttpPost("users")]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest createUserRequest)
        {
            var result = await _usersService.CreateAsync(createUserRequest);

            if (result == null) return StatusCode(500);

            return Ok(result);
        }
    }
}
