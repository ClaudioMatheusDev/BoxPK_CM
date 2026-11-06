using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using WebApi.Seed;

namespace BoxPKCM.Tests
{
    public class DemoDataSeederTests
    {
        [Fact]
        public async Task Popula_catalogo_pokemon_com_relacionamentos()
        {
            using var db = new TestDb();
            await using var ctx = db.CreateContext();

            Assert.True(await DemoDataSeeder.SeedAsync(ctx));

            Assert.Equal(1, await ctx.Jogos.CountAsync());
            Assert.True(await ctx.Colecoes.CountAsync() >= 10);
            Assert.True(await ctx.Fornecedores.CountAsync() >= 3);
            Assert.True(await ctx.Produtos.CountAsync(p => p.Categoria.Nome == "Carta Avulsa") >= 20);
            Assert.Equal(await ctx.Produtos.CountAsync(), await ctx.Estoques.CountAsync());
            Assert.Contains(await ctx.Compras.Select(c => c.StatusCompra).Distinct().ToListAsync(), s => s == StatusCompra.Pendente);
        }

        [Fact]
        public async Task Segunda_execucao_nao_duplica_dados()
        {
            using var db = new TestDb();
            await using (var ctx = db.CreateContext())
                Assert.True(await DemoDataSeeder.SeedAsync(ctx));

            await using var ctx2 = db.CreateContext();
            var produtos = await ctx2.Produtos.CountAsync();

            Assert.False(await DemoDataSeeder.SeedAsync(ctx2));
            Assert.Equal(produtos, await ctx2.Produtos.CountAsync());
        }

        [Fact]
        public async Task Dados_ficam_isolados_no_tenant_do_seed()
        {
            using var db = new TestDb();
            await using (var ctx = db.CreateContext())
                await DemoDataSeeder.SeedAsync(ctx);

            db.Tenant.TenantId = Guid.NewGuid();
            await using var outroTenant = db.CreateContext();

            Assert.Equal(0, await outroTenant.Produtos.CountAsync());
        }

        [Fact]
        public async Task Estoque_e_status_sao_coerentes_com_compras_e_movimentacoes()
        {
            using var db = new TestDb();
            await using (var seed = db.CreateContext())
                await DemoDataSeeder.SeedAsync(seed);

            await using var ctx = db.CreateContext();
            var estoques = await ctx.Estoques.Include(e => e.Produto).ToListAsync();
            var movimentacoes = await ctx.MovimentacaoEstoques.ToListAsync();
            var compras = await ctx.Compras.Include(c => c.Itens).ToListAsync();

            foreach (var estoque in estoques)
            {
                var saldo = movimentacoes
                    .Where(m => m.IDProduto == estoque.IDProduto)
                    .Sum(m => m.TipoMovimentacao == TipoMovimentacao.Entrada ? m.Quantidade : -m.Quantidade);

                Assert.Equal(saldo, estoque.QuantidadeAtual);
                Assert.True(estoque.QuantidadeAtual >= 0);
                Assert.Equal(estoque.QuantidadeAtual > 0, estoque.Produto.StatusProduto == StatusProduto.Ativo);
            }

            // Entradas só vêm de compras confirmadas, e o total da compra bate com os itens.
            var entradas = movimentacoes.Where(m => m.TipoMovimentacao == TipoMovimentacao.Entrada).Sum(m => m.Quantidade);
            var recebido = compras.Where(c => c.StatusCompra == StatusCompra.Confirmada).SelectMany(c => c.Itens).Sum(i => i.Quantidade);
            Assert.Equal(recebido, entradas);
            Assert.All(compras, c => Assert.Equal(c.Itens.Sum(i => i.ValorTotal), c.ValorTotal));

            Assert.Contains(estoques, e => e.Produto.StatusProduto == StatusProduto.AguardandoEntrega);
            Assert.Contains(estoques, e => e.Produto.StatusProduto == StatusProduto.ForaEstoque);
        }
    }
}
