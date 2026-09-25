using System.Security.Claims;
using Candiate_search_assesment.Model.Payload;
using Candiate_search_assesment.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Candiate_search_assesment.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request, CancellationToken cancellationToken)
        {
            var result = await _authService.RegisterAsync(request, cancellationToken);

            if (!result.Success)
            {
                return result.IsConflict
                    ? Conflict(new { message = result.ErrorMessage })
                    : BadRequest(new { message = result.ErrorMessage });
            }

            return StatusCode(StatusCodes.Status201Created, new
            {
                message = "Registration successful.",
                candidate = result.Candidate
            });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
        {
            var result = await _authService.LoginAsync(request, cancellationToken);

            if (result is null)
            {
             
                return Unauthorized(new { message = "Invalid email or password." });
            }

            return Ok(result);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto request, CancellationToken cancellationToken)
        {
            var result = await _authService.RefreshAsync(request, cancellationToken);

            if (result is null)
            {
                return Unauthorized(new { message = "Invalid or expired refresh token." });
            }

            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            var candidateIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(candidateIdClaim, out var candidateId))
            {
                await _authService.LogoutAsync(candidateId, cancellationToken);
            }

            return NoContent();
        }
    }
}
