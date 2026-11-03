using Application.Dtos;
using Application.Users;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Service.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService,
            AppDbContext db,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _db = db;
            _configuration = configuration;
        }

        public int RefreshTokenExpirationDays =>
            int.TryParse(_configuration["Jwt:RefreshTokenExpirationDays"], out var days) && days > 0 ? days : 7;

        public Task<IdentityResult> RegisterAsync(RegisterDto dto)
        {
            // Cada usuário recebe um tenant próprio no cadastro. Nunca aceitar TenantId do cliente.
            var user = new ApplicationUser { UserName = dto.Email, Email = dto.Email, TenantId = Guid.NewGuid() };
            return _userManager.CreateAsync(user, dto.Password);
        }

        public async Task<AuthResult> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null)
                return new AuthResult(AuthStatus.InvalidCredentials);

            if (await _userManager.IsLockedOutAsync(user))
                return new AuthResult(AuthStatus.LockedOut);

            if (!await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                // Conta falhas e bloqueia temporariamente após o limite configurado.
                await _userManager.AccessFailedAsync(user);
                return new AuthResult(await _userManager.IsLockedOutAsync(user)
                    ? AuthStatus.LockedOut
                    : AuthStatus.InvalidCredentials);
            }

            await _userManager.ResetAccessFailedCountAsync(user);
            return await IssueTokensAsync(user);
        }

        public async Task<AuthResult> RefreshAsync(string incomingRefreshToken)
        {
            var hash = _tokenService.ComputeRefreshTokenHash(incomingRefreshToken);
            var stored = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == hash);
            if (stored is null)
                return new AuthResult(AuthStatus.InvalidCredentials);

            if (stored.Revoked)
            {
                // Reuso de um token já rotacionado indica possível roubo: invalida toda a sessão do usuário.
                var active = await _db.RefreshTokens.Where(x => x.UserId == stored.UserId && !x.Revoked).ToListAsync();
                active.ForEach(x => x.Revoked = true);
                await _db.SaveChangesAsync();
                return new AuthResult(AuthStatus.InvalidCredentials);
            }

            if (stored.ExpiresAt < DateTime.UtcNow)
                return new AuthResult(AuthStatus.InvalidCredentials);

            var user = await _userManager.FindByIdAsync(stored.UserId.ToString());
            if (user is null || await _userManager.IsLockedOutAsync(user))
                return new AuthResult(AuthStatus.InvalidCredentials);

            stored.Revoked = true; // rotação
            return await IssueTokensAsync(user);
        }

        public async Task LogoutAsync(string incomingRefreshToken)
        {
            var hash = _tokenService.ComputeRefreshTokenHash(incomingRefreshToken);
            var stored = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == hash && !x.Revoked);
            if (stored is null) return;

            stored.Revoked = true;
            await _db.SaveChangesAsync();
        }

        private async Task<AuthResult> IssueTokensAsync(ApplicationUser user)
        {
            if (user.TenantId == Guid.Empty)
            {
                user.TenantId = Guid.NewGuid();
                var update = await _userManager.UpdateAsync(user);
                if (!update.Succeeded)
                    throw new InvalidOperationException("Não foi possível atribuir um tenant ao usuário.");
            }

            var access = _tokenService.CreateAccessToken(user);
            var (refresh, raw) = _tokenService.CreateRefreshToken(user.Id, RefreshTokenExpirationDays);
            _db.RefreshTokens.Add(refresh);
            await _db.SaveChangesAsync();

            return new AuthResult(AuthStatus.Success, access, refresh, raw);
        }
    }
}
