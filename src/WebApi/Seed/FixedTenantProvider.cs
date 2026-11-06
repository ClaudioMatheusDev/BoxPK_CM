using Application.Common;

namespace WebApi.Seed
{
    /// <summary>Tenant fixo para operações fora de uma requisição HTTP (ex.: seeds na inicialização).</summary>
    public sealed class FixedTenantProvider : ITenantProvider
    {
        public FixedTenantProvider(Guid tenantId) => TenantId = tenantId;

        public Guid TenantId { get; }
    }
}
