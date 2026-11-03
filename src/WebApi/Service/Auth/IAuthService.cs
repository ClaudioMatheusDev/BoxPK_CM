using Application.Dtos;
using Application.Users;
using Microsoft.AspNetCore.Identity;

namespace WebApi.Service.Auth
{
    public enum AuthStatus
    {
        Success,
        InvalidCredentials,
        LockedOut
    }

    public record AuthResult(AuthStatus Status, string? AccessToken = null, RefreshToken? RefreshToken = null, string? RawRefreshToken = null);

    public interface IAuthService
    {
        Task<IdentityResult> RegisterAsync(RegisterDto dto);
        Task<AuthResult> LoginAsync(LoginDto dto);
        Task<AuthResult> RefreshAsync(string incomingRefreshToken);
        Task LogoutAsync(string incomingRefreshToken);
        int RefreshTokenExpirationDays { get; }
    }
}
