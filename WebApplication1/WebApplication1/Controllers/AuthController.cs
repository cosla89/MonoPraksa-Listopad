using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zoo.model;
using Zoo.service;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            var error = _authService.Register(request);

            if (error != null)
            {
                return BadRequest(new { message = error });
            }

            return StatusCode(201, new
            {
                message = "User has been added"
            });
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var response = _authService.Login(request);

            if (response == null)
            {
                return Unauthorized(new
                {
                    message = "Wronf username or password."
                });
            }

            return Ok(response);
        }
    }
}