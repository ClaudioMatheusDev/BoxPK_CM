using Application.Users;

namespace WebApi.Service
{
    public interface ITokenService
    {
        string CreateAccessToken(ApplicationUser user);
        // Retorna o par (RefreshToken salvo, RawToken) onde o RefreshToken.Token contém o hash salvo no banco
        (RefreshToken RefreshToken, string RawToken) CreateRefreshToken(Guid userId, int days);

        // Computa o hash do refresh token (usado para comparar tokens recebidos)
        string ComputeRefreshTokenHash(string token);
    }
}
