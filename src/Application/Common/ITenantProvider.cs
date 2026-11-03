namespace Application.Common
{
    /// <summary>Fornece o tenant do usuário autenticado na requisição atual.</summary>
    public interface ITenantProvider
    {
        /// <summary>TenantId do usuário atual, ou Guid.Empty quando não autenticado.</summary>
        Guid TenantId { get; }
    }
}
