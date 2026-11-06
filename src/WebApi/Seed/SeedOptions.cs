namespace WebApi.Seed
{
    /// <summary>
    /// Configuração dos seeds (seção "Seed"). Exemplo via variáveis de ambiente:
    /// Seed__Enabled=true, Seed__AdminEmail=admin@boxtcg.local, Seed__AdminPassword=..., Seed__DemoData=true.
    /// </summary>
    public class SeedOptions
    {
        public const string SectionName = "Seed";

        /// <summary>Liga a execução dos seeds na inicialização da API.</summary>
        public bool Enabled { get; set; }

        /// <summary>E-mail (e nome de usuário) do administrador padrão.</summary>
        public string AdminEmail { get; set; } = "admin@boxtcg.local";

        /// <summary>Senha do administrador padrão. Obrigatória quando os seeds estão ligados; nunca versionar.</summary>
        public string? AdminPassword { get; set; }

        /// <summary>Popula o tenant do administrador com o catálogo de demonstração (cartas Pokémon, compras, estoque).</summary>
        public bool DemoData { get; set; } = true;
    }
}
