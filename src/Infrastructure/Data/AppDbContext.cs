using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Colecao> Colecoes { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<ItemCompra> ItensCompras { get; set; }
        public DbSet<Estoque> Estoques { get; set; }
        public DbSet<MovimentacaoEstoque> MovimentacaoEstoques { get; set; }
        public DbSet<Fornecedor> Fornecedores { get; set; }
        public DbSet<Jogo> Jogos { get; set; }
        public DbSet<Produto> Produtos { get; set; }

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
        }
    }
}
