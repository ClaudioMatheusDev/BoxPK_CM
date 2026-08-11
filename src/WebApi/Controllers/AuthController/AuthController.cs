using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Application.Users;
using Microsoft.EntityFrameworkCore;
using WebApi.Service;
using Infrastructure.Data;
using Application.Dtos;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            AppDbContext db,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _db = db;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            // Cada usuário recebe um tenant próprio no cadastro. Não aceite TenantId do cliente.
            var user = new ApplicationUser { UserName = dto.Email, Email = dto.Email, TenantId = Guid.NewGuid() };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return Unauthorized();

            var pw = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!pw) return Unauthorized();

            if (user.TenantId == Guid.Empty)
            {
                user.TenantId = Guid.NewGuid();
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded) return BadRequest(updateResult.Errors);
            }

            var access = _tokenService.CreateAccessToken(user);
            var (refresh, raw) = _tokenService.CreateRefreshToken(user.Id, GetRefreshTokenExpirationDays());
            _db.RefreshTokens.Add(refresh);
            await _db.SaveChangesAsync();

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Strict,
                Expires = refresh.ExpiresAt
            };
            Response.Cookies.Append("ft_refresh", raw, cookieOptions);

            return Ok(new { accessToken = access });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshDto dto)
        {
            // Prefer cookie; fall back to body if provided
            var incoming = string.Empty;
            if (Request.Cookies.ContainsKey("ft_refresh")) incoming = Request.Cookies["ft_refresh"]!;
            if (string.IsNullOrWhiteSpace(incoming) && !string.IsNullOrWhiteSpace(dto.RefreshToken)) incoming = dto.RefreshToken;

            if (string.IsNullOrWhiteSpace(incoming)) return Unauthorized();

            var hash = _tokenService.ComputeRefreshTokenHash(incoming);
            var rt = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == hash && !x.Revoked);
            if (rt == null || rt.ExpiresAt < DateTime.UtcNow) return Unauthorized();

            var user = await _userManager.FindByIdAsync(rt.UserId.ToString());
            if (user == null) return Unauthorized();

            if (user.TenantId == Guid.Empty)
            {
                user.TenantId = Guid.NewGuid();
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded) return BadRequest(updateResult.Errors);
            }

            rt.Revoked = true; // rotate
            var (newRefresh, newRaw) = _tokenService.CreateRefreshToken(user.Id, GetRefreshTokenExpirationDays());
            _db.RefreshTokens.Add(newRefresh);
            await _db.SaveChangesAsync();

            // replace cookie
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Strict,
                Expires = newRefresh.ExpiresAt
            };
            Response.Cookies.Append("ft_refresh", newRaw, cookieOptions);

            var access = _tokenService.CreateAccessToken(user);
            return Ok(new { accessToken = access });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            if (Request.Cookies.TryGetValue("ft_refresh", out var raw) && !string.IsNullOrWhiteSpace(raw))
            {
                var hash = _tokenService.ComputeRefreshTokenHash(raw);
                var rt = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == hash && !x.Revoked);
                if (rt != null)
                {
                    rt.Revoked = true;
                    await _db.SaveChangesAsync();
                }
            }

            Response.Cookies.Delete("ft_refresh", new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Strict
            });
            return NoContent();
        }

        private int GetRefreshTokenExpirationDays()
        {
            var configuredValue = _configuration["Jwt:RefreshTokenExpirationDays"];
            return int.TryParse(configuredValue, out var days) && days > 0 ? days : 7;
        }
    }
}
