using Application.Common;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BoxPKCM.Tests
{
    public class FakeTenantProvider : ITenantProvider
    {
        public Guid TenantId { get; set; } = Guid.NewGuid();
    }

    /// <summary>Banco SQLite em memória com o modelo real do AppDbContext (filtros de tenant incluídos).</summary>
    public sealed class TestDb : IDisposable
    {
        private readonly SqliteConnection _connection;

        public FakeTenantProvider Tenant { get; } = new();

        public TestDb()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            using var context = CreateContext();
            context.Database.EnsureCreated();
        }

        public AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options;
            return new AppDbContext(options, Tenant);
        }

        /// <summary>Cria categoria, jogo, fornecedor e um produto com estoque inicial.</summary>
        public async Task<(int IdProduto, int IdFornecedor)> SeedAsync(int saldoInicial = 0)
        {
            await using var context = CreateContext();

            var categoria = new Categoria { Nome = "Booster", StatusCategoria = StatusCategoria.Ativo };
            var jogo = new Jogo { Nome = "Pokemon", StatusJogo = StatusJogo.Ativo };
            var fornecedor = new Fornecedor
            {
                Nome = "Fornecedor", CPFCNPJ = "123", Email = "f@f.com", Telefone = "1", StatusFornecedor = StatusFornecedor.Ativo
            };
            context.AddRange(categoria, jogo, fornecedor);
            await context.SaveChangesAsync();

            var produto = new Produto
            {
                Nome = "Box",
                IDCategoria = categoria.IDCategoria,
                IDJogoTCG = jogo.IDJogoTCG,
                PrecoCusto = 10,
                PrecoVenda = 20,
                StatusProduto = StatusProduto.Ativo
            };
            context.Produtos.Add(produto);
            await context.SaveChangesAsync();

            context.Estoques.Add(new Estoque { IDProduto = produto.IDProduto, QuantidadeAtual = saldoInicial });
            await context.SaveChangesAsync();

            return (produto.IDProduto, fornecedor.IDFornecedor);
        }

        public void Dispose() => _connection.Dispose();
    }
}
