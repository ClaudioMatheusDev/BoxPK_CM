using Application.Dtos;
using Application.Exceptions;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using WebApi.Service;

namespace BoxPKCM.Tests
{
    public class MovimentacaoEstoqueTests
    {
        [Fact]
        public async Task Saida_maior_que_o_saldo_e_rejeitada()
        {
            using var db = new TestDb();
            var (produto, _) = await db.SeedAsync(saldoInicial: 5);
            await using var ctx = db.CreateContext();
            var service = new MovimentacaoEstoqueService(ctx);

            await Assert.ThrowsAsync<BusinessException>(() => service.CriarMovimentacaoEstoque(
                new MovimentacaoEstoqueCriarDto { IDProduto = produto, TipoMovimentacao = TipoMovimentacao.Saida, Quantidade = 6 }));
        }

        [Fact]
        public async Task Quantidade_zero_ou_negativa_e_rejeitada()
        {
            using var db = new TestDb();
            var (produto, _) = await db.SeedAsync(saldoInicial: 5);
            await using var ctx = db.CreateContext();
            var service = new MovimentacaoEstoqueService(ctx);

            await Assert.ThrowsAsync<BusinessException>(() => service.CriarMovimentacaoEstoque(
                new MovimentacaoEstoqueCriarDto { IDProduto = produto, TipoMovimentacao = TipoMovimentacao.Entrada, Quantidade = 0 }));
        }

        [Fact]
        public async Task Entrada_e_saida_atualizam_saldo_e_registram_historico()
        {
            using var db = new TestDb();
            var (produto, _) = await db.SeedAsync(saldoInicial: 2);
            await using var ctx = db.CreateContext();
            var service = new MovimentacaoEstoqueService(ctx);

            await service.CriarMovimentacaoEstoque(new MovimentacaoEstoqueCriarDto { IDProduto = produto, TipoMovimentacao = TipoMovimentacao.Entrada, Quantidade = 10 });
            var idSaida = await service.CriarMovimentacaoEstoque(new MovimentacaoEstoqueCriarDto { IDProduto = produto, TipoMovimentacao = TipoMovimentacao.Saida, Quantidade = 4 });

            var estoque = await ctx.Estoques.AsNoTracking().SingleAsync();
            Assert.Equal(8, estoque.QuantidadeAtual);

            var saida = await service.ListarMovimentacaoPorID(idSaida);
            Assert.Equal(12, saida.QuantidadeAnterior);
            Assert.Equal(8, saida.QuantidadePosterior);
        }

        [Fact]
        public async Task Excluir_entrada_que_deixaria_saldo_negativo_e_rejeitado()
        {
            using var db = new TestDb();
            var (produto, _) = await db.SeedAsync(saldoInicial: 0);
            await using var ctx = db.CreateContext();
            var service = new MovimentacaoEstoqueService(ctx);

            var idEntrada = await service.CriarMovimentacaoEstoque(new MovimentacaoEstoqueCriarDto { IDProduto = produto, TipoMovimentacao = TipoMovimentacao.Entrada, Quantidade = 5 });
            await service.CriarMovimentacaoEstoque(new MovimentacaoEstoqueCriarDto { IDProduto = produto, TipoMovimentacao = TipoMovimentacao.Saida, Quantidade = 5 });

            await Assert.ThrowsAsync<BusinessException>(() => service.DeletarMovimentacaoEstoque(idEntrada));
        }
    }

    public class CompraTests
    {
        [Fact]
        public async Task Valor_total_vem_dos_itens_e_nao_do_cliente()
        {
            using var db = new TestDb();
            var (produto, fornecedor) = await db.SeedAsync();
            await using var ctx = db.CreateContext();
            var compras = new CompraService(ctx);
            var itens = new ItemCompraService(ctx);

            var idCompra = await compras.CriarCompra(new CompraCriarDto { IDFornecedor = fornecedor, DataCompra = DateTime.Today, ValorTotal = 9999 });
            await itens.CriarItemCompra(new ItemCompraCriarDto { IDCompra = idCompra, IDProduto = produto, Quantidade = 3, ValorUnitario = 10 });
            await itens.CriarItemCompra(new ItemCompraCriarDto { IDCompra = idCompra, IDProduto = produto, Quantidade = 2, ValorUnitario = 5 });

            Assert.Equal(40m, (await compras.ListarCompraPorID(idCompra)).ValorTotal);
        }

        [Fact]
        public async Task Item_com_produto_inexistente_e_rejeitado()
        {
            using var db = new TestDb();
            var (_, fornecedor) = await db.SeedAsync();
            await using var ctx = db.CreateContext();
            var compras = new CompraService(ctx);
            var itens = new ItemCompraService(ctx);

            var idCompra = await compras.CriarCompra(new CompraCriarDto { IDFornecedor = fornecedor, DataCompra = DateTime.Today });

            await Assert.ThrowsAsync<NotFoundException>(() => itens.CriarItemCompra(
                new ItemCompraCriarDto { IDCompra = idCompra, IDProduto = 999, Quantidade = 1, ValorUnitario = 1 }));
        }

        [Fact]
        public async Task Confirmar_compra_gera_entrada_no_estoque_e_trava_alteracoes()
        {
            using var db = new TestDb();
            var (produto, fornecedor) = await db.SeedAsync(saldoInicial: 1);
            await using var ctx = db.CreateContext();
            var compras = new CompraService(ctx);
            var itens = new ItemCompraService(ctx);

            var idCompra = await compras.CriarCompra(new CompraCriarDto { IDFornecedor = fornecedor, DataCompra = DateTime.Today });
            await itens.CriarItemCompra(new ItemCompraCriarDto { IDCompra = idCompra, IDProduto = produto, Quantidade = 7, ValorUnitario = 2 });

            await compras.AtualizarCompraPorID(idCompra, new CompraAtualizarDto
            {
                IDFornecedor = fornecedor, DataCompra = DateTime.Today, StatusCompra = StatusCompra.Confirmada
            });

            Assert.Equal(8, (await ctx.Estoques.AsNoTracking().SingleAsync()).QuantidadeAtual);
            Assert.Single(await ctx.MovimentacaoEstoques.AsNoTracking().ToListAsync());

            await Assert.ThrowsAsync<BusinessException>(() => itens.CriarItemCompra(
                new ItemCompraCriarDto { IDCompra = idCompra, IDProduto = produto, Quantidade = 1, ValorUnitario = 1 }));
            await Assert.ThrowsAsync<BusinessException>(() => compras.DeletarCompra(idCompra));
        }
    }

    public class TenantIsolationTests
    {
        [Fact]
        public async Task Tenant_nao_enxerga_dados_de_outro_tenant()
        {
            using var db = new TestDb();
            await db.SeedAsync();

            await using (var ctxA = db.CreateContext())
                Assert.Single(await ctxA.Produtos.ToListAsync());

            db.Tenant.TenantId = Guid.NewGuid(); // outro usuário/tenant

            await using var ctxB = db.CreateContext();
            Assert.Empty(await ctxB.Produtos.ToListAsync());
            Assert.Empty(await ctxB.Fornecedores.ToListAsync());
            await Assert.ThrowsAsync<NotFoundException>(() => new ProdutoService(ctxB).ListarProdutoPorID(1));
        }

        [Fact]
        public async Task Gravar_sem_tenant_e_bloqueado()
        {
            using var db = new TestDb();
            db.Tenant.TenantId = Guid.Empty;

            await using var ctx = db.CreateContext();
            ctx.Categorias.Add(new Domain.Entities.Categoria { Nome = "X", StatusCategoria = StatusCategoria.Ativo });

            await Assert.ThrowsAsync<InvalidOperationException>(() => ctx.SaveChangesAsync());
        }

        [Fact]
        public async Task Cadastro_de_usuario_funciona_sem_tenant_na_requisicao()
        {
            using var db = new TestDb();
            db.Tenant.TenantId = Guid.Empty; // registro é anônimo: ainda não há claim de tenant

            await using var ctx = db.CreateContext();
            ctx.Users.Add(new Application.Users.ApplicationUser { UserName = "a@a.com", Email = "a@a.com", TenantId = Guid.NewGuid() });

            await ctx.SaveChangesAsync();
            Assert.Equal(1, await ctx.Users.CountAsync());
        }
    }
}
