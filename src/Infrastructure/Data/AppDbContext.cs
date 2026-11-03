using Application.Common;
using Application.Users;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        private static readonly string TenantProperty = "TenantId";
        private static readonly HashSet<Type> TenantEntityTypes = new();
        private readonly ITenantProvider _tenantProvider;

        public AppDbContext(DbContextOptions<AppDbContext> options, ITenantProvider tenantProvider) : base(options)
        {
            _tenantProvider = tenantProvider;
        }

        /// <summary>TenantId atual, usado pelos filtros globais de consulta (avaliado por instância do contexto).</summary>
        private Guid CurrentTenantId => _tenantProvider.TenantId;

        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Colecao> Colecoes { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<ItemCompra> ItensCompras { get; set; }
        public DbSet<Estoque> Estoques { get; set; }
        public DbSet<MovimentacaoEstoque> MovimentacaoEstoques { get; set; }
        public DbSet<Fornecedor> Fornecedores { get; set; }
        public DbSet<Jogo> Jogos { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Produto>(entity =>
            {
                entity.Property(p => p.PrecoCusto).HasPrecision(18, 2);
                entity.Property(p => p.PrecoVenda).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Compra>()
                .Property(c => c.ValorTotal)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ItemCompra>(entity =>
            {
                entity.Property(i => i.ValorUnitario).HasPrecision(18, 2);
                entity.Property(i => i.ValorTotal).HasPrecision(18, 2);
            });

            modelBuilder.Entity<MovimentacaoEstoque>()
                .HasOne(m => m.Produto)
                .WithMany(p => p.MovimentacoesEstoque)
                .HasForeignKey(m => m.IDProduto)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Estoque>()
                .HasOne(e => e.Produto)
                .WithOne(p => p.Estoque)
                .HasForeignKey<Estoque>(e => e.IDProduto)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Produto>()
                .HasOne(p => p.Categoria)
                .WithMany(c => c.Produtos)
                .HasForeignKey(p => p.IDCategoria)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Produto>()
                .HasOne(p => p.JogoTCG)
                .WithMany(j => j.Produtos)
                .HasForeignKey(p => p.IDJogoTCG)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Colecao>()
                .HasOne(c => c.JogoTCG)
                .WithMany(j => j.Colecoes)
                .HasForeignKey(c => c.IDJogoTCG)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Produto>()
                .HasOne(p => p.Colecao)
                .WithMany(c => c.Produtos)
                .HasForeignKey(p => p.IDColecao)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Compra>()
                .HasOne(c => c.Fornecedor)
                .WithMany(f => f.Compras)
                .HasForeignKey(c => c.IDFornecedor)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ItemCompra>()
                .HasOne(i => i.Compra)
                .WithMany(c => c.Itens)
                .HasForeignKey(i => i.IDCompra)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ItemCompra>()
                .HasOne(i => i.Produto)
                .WithMany(p => p.ItensCompra)
                .HasForeignKey(i => i.IDProduto)
                .OnDelete(DeleteBehavior.Restrict);

            ConfigurarTenant<Categoria>(modelBuilder);
            ConfigurarTenant<Colecao>(modelBuilder);
            ConfigurarTenant<Compra>(modelBuilder);
            ConfigurarTenant<ItemCompra>(modelBuilder);
            ConfigurarTenant<Estoque>(modelBuilder);
            ConfigurarTenant<MovimentacaoEstoque>(modelBuilder);
            ConfigurarTenant<Fornecedor>(modelBuilder);
            ConfigurarTenant<Jogo>(modelBuilder);
            ConfigurarTenant<Produto>(modelBuilder);

            modelBuilder.Entity<Estoque>()
                .Property(e => e.RowVersion)
                .IsRowVersion();
        }

        /// <summary>
        /// Isolamento multi-tenant: cada entidade de negócio ganha uma coluna de sombra TenantId
        /// e um filtro global que só devolve registros do tenant do usuário autenticado.
        /// </summary>
        private void ConfigurarTenant<TEntity>(ModelBuilder modelBuilder) where TEntity : class
        {
            TenantEntityTypes.Add(typeof(TEntity));
            modelBuilder.Entity<TEntity>().Property<Guid>(TenantProperty);
            modelBuilder.Entity<TEntity>().HasIndex(TenantProperty);
            modelBuilder.Entity<TEntity>().HasQueryFilter(e => EF.Property<Guid>(e, TenantProperty) == CurrentTenantId);
        }

        private void AplicarTenant()
        {
            foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
            {
                // Só entidades de negócio: ApplicationUser (Identity) possui a própria propriedade TenantId.
                if (!TenantEntityTypes.Contains(entry.Entity.GetType())) continue;

                if (CurrentTenantId == Guid.Empty)
                    throw new InvalidOperationException("Operação exige um usuário autenticado com TenantId.");

                entry.Property(TenantProperty).CurrentValue = CurrentTenantId;
            }
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            AplicarTenant();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            AplicarTenant();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
