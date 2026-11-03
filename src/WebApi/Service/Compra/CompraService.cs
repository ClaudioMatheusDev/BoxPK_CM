using Application.Common;
using Application.Dtos;
using Application.Exceptions;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Service
{
    public class CompraService : ICompraService
    {
        private readonly AppDbContext _context;

        public CompraService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> CriarCompra(CompraCriarDto dto)
        {
            await GarantirFornecedor(dto.IDFornecedor);

            // O valor total é sempre derivado dos itens; nunca aceito do cliente.
            var compra = new Compra
            {
                IDFornecedor = dto.IDFornecedor,
                DataCompra = dto.DataCompra,
                ValorTotal = 0,
                Observacao = dto.Observacao,
                StatusCompra = StatusCompra.Pendente
            };

            _context.Compras.Add(compra);
            await _context.SaveChangesAsync();

            return compra.IDCompra;
        }

        public async Task<List<Compra>> ListarCompras(int? page = null, int? pageSize = null)
        {
            var (skip, take) = PagedQuery.Normalize(page, pageSize);
            return await _context.Compras.AsNoTracking().OrderBy(c => c.IDCompra).Skip(skip).Take(take).ToListAsync();
        }

        public async Task<Compra> ListarCompraPorID(int IDCompra)
        {
            var compra = await _context.Compras.AsNoTracking().FirstOrDefaultAsync(c => c.IDCompra == IDCompra);

            return compra ?? throw new NotFoundException("Nenhuma compra encontrada com esse ID!");
        }

        public async Task<int> AtualizarCompraPorID(int IDCompra, CompraAtualizarDto dto)
        {
            var compra = await _context.Compras.Include(c => c.Itens).FirstOrDefaultAsync(c => c.IDCompra == IDCompra)
                ?? throw new NotFoundException("Nenhuma compra encontrada com esse ID!");

            var statusAnterior = compra.StatusCompra;

            if (statusAnterior != StatusCompra.Pendente && dto.StatusCompra != statusAnterior)
            {
                throw new BusinessException("Somente compras pendentes podem mudar de status.");
            }

            if (statusAnterior != StatusCompra.Pendente &&
                (dto.IDFornecedor != compra.IDFornecedor || dto.DataCompra != compra.DataCompra))
            {
                throw new BusinessException("Compras confirmadas ou canceladas não podem ser alteradas.");
            }

            await GarantirFornecedor(dto.IDFornecedor);

            compra.IDFornecedor = dto.IDFornecedor;
            compra.DataCompra = dto.DataCompra;
            compra.Observacao = dto.Observacao;
            compra.ValorTotal = compra.Itens.Sum(i => i.ValorTotal);

            await using var transaction = await _context.Database.BeginTransactionAsync();

            if (statusAnterior == StatusCompra.Pendente && dto.StatusCompra == StatusCompra.Confirmada)
            {
                await ReceberItensNoEstoque(compra);
            }

            compra.StatusCompra = dto.StatusCompra;

            await SalvarComConcorrencia();
            await transaction.CommitAsync();

            return compra.IDCompra;
        }

        public async Task<bool> DeletarCompra(int IDCompra)
        {
            var compra = await _context.Compras.FirstOrDefaultAsync(c => c.IDCompra == IDCompra)
                ?? throw new NotFoundException("Nenhuma compra encontrada com esse ID!");

            if (compra.StatusCompra == StatusCompra.Confirmada)
            {
                throw new BusinessException("Compras confirmadas já geraram entrada no estoque e não podem ser excluídas.");
            }

            _context.Compras.Remove(compra);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>Confirmar a compra gera uma movimentação de entrada para cada item.</summary>
        private async Task ReceberItensNoEstoque(Compra compra)
        {
            var produtoIds = compra.Itens.Select(i => i.IDProduto).Distinct().ToList();
            var estoques = await _context.Estoques.Where(e => produtoIds.Contains(e.IDProduto)).ToDictionaryAsync(e => e.IDProduto);

            foreach (var item in compra.Itens)
            {
                if (!estoques.TryGetValue(item.IDProduto, out var estoque))
                {
                    estoque = new Estoque { IDProduto = item.IDProduto, DataAtualizacao = DateTime.Now };
                    _context.Estoques.Add(estoque);
                    estoques[item.IDProduto] = estoque;
                }

                var movimentacao = MovimentacaoEstoqueService.AplicarMovimentacao(
                    estoque, TipoMovimentacao.Entrada, item.Quantidade,
                    "Recebimento de compra", $"Compra #{compra.IDCompra}");

                _context.MovimentacaoEstoques.Add(movimentacao);
            }
        }

        private async Task GarantirFornecedor(int idFornecedor)
        {
            if (!await _context.Fornecedores.AnyAsync(f => f.IDFornecedor == idFornecedor))
            {
                throw new NotFoundException("Fornecedor não encontrado.");
            }
        }

        private async Task SalvarComConcorrencia()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConflictException("O estoque foi alterado por outra operação. Tente novamente.");
            }
        }
    }
}
