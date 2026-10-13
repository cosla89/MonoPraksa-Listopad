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
            public async Task<IActionResult> Register(
            [FromBody] RegisterRequest request)
        {
            var error = await _authService.RegisterAsync(request);

            if (error != null)
            {
                return BadRequest(new { message = error });
            }

            return StatusCode(201, new
            {
                message = "Korisnik je uspješno registriran."
            });
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(
    [FromBody] LoginRequest request)
        {
            var response = await _authService.LoginAsync(request);

            if (response == null)
            {
                return Unauthorized(new
                {
                    message = "Neispravno korisničko ime ili lozinka."
                });
            }

            return Ok(response);
        }
    }
}