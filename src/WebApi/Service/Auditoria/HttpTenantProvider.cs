using Application.Common;

namespace WebApi.Service.Auditoria
{
    /// <summary>Lê o claim TenantId do usuário autenticado na requisição atual.</summary>
    public class HttpTenantProvider : ITenantProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpTenantProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid TenantId =>
            Guid.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst("TenantId")?.Value, out var id)
                ? id
                : Guid.Empty;
    }
}
