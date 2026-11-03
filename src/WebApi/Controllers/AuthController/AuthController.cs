using Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WebApi.Service.Auth;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    public class AuthController : ControllerBase
    {
        private const string RefreshCookie = "ft_refresh";

        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            // Cadastro aberto por padrão; defina Auth:AllowRegistration=false para desativar.
            if (!_configuration.GetValue("Auth:AllowRegistration", true))
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "O cadastro de novos usuários está desativado." });

            var result = await _authService.RegisterAsync(dto);
            if (!result.Succeeded) return BadRequest(result.Errors);
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            return ToResponse(result);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshDto dto)
        {
            // Prefere o cookie; usa o corpo da requisição como alternativa.
            var incoming = Request.Cookies.TryGetValue(RefreshCookie, out var cookie) ? cookie : null;
            if (string.IsNullOrWhiteSpace(incoming)) incoming = dto.RefreshToken;
            if (string.IsNullOrWhiteSpace(incoming)) return Unauthorized();

            var result = await _authService.RefreshAsync(incoming);
            return ToResponse(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            if (Request.Cookies.TryGetValue(RefreshCookie, out var raw) && !string.IsNullOrWhiteSpace(raw))
            {
                await _authService.LogoutAsync(raw);
            }

            Response.Cookies.Delete(RefreshCookie, BuildCookieOptions(null));
            return NoContent();
        }

        private IActionResult ToResponse(AuthResult result)
        {
            switch (result.Status)
            {
                case AuthStatus.LockedOut:
                    return StatusCode(StatusCodes.Status429TooManyRequests,
                        new { message = "Muitas tentativas inválidas. Tente novamente mais tarde." });
                case AuthStatus.Success:
                    Response.Cookies.Append(RefreshCookie, result.RawRefreshToken!, BuildCookieOptions(result.RefreshToken!.ExpiresAt));
                    return Ok(new { accessToken = result.AccessToken });
                default:
                    return Unauthorized();
            }
        }

        private CookieOptions BuildCookieOptions(DateTime? expires) => new()
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Strict,
            Expires = expires
        };
    }
}
