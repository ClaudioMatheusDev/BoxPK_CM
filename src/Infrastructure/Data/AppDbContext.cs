using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Categoria> Categorias { get; set;}
        public DbSet<Colecao> Colecoes { get; set;}
        public DbSet<Compra> Compras { get; set;}
        public DbSet<ItemCompra> ItensCompras { get; set;}
        public DbSet<Estoque> Estoques { get; set;}
        public DbSet<MovimentacaoEstoque> MovimentacaoEstoques { get; set; }
        public DbSet<Fornecedor> Fornecedores { get; set; }
        public DbSet<Jogo> Jogos { get; set; }
        public DbSet<Produto> Produtos { get; set;}

    }
}
