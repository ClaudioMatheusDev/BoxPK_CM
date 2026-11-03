using Application.Dtos;
using Application.Exceptions;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Service
{
    public class MovimentacaoEstoqueService : IMovimentacaoEstoqueService
    {
        private readonly AppDbContext _context;

        public MovimentacaoEstoqueService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> CriarMovimentacaoEstoque(MovimentacaoEstoqueCriarDto dto)
        {
            if (dto.Quantidade <= 0)
            {
                throw new BusinessException("A quantidade da movimentação deve ser maior que zero.");
            }

            var estoque = await _context.Estoques.FirstOrDefaultAsync(e => e.IDProduto == dto.IDProduto);

            if (estoque is null)
            {
                throw new NotFoundException("Estoque não encontrado.");
            }

            var movimentacao = AplicarMovimentacao(estoque, dto.TipoMovimentacao, dto.Quantidade, dto.Motivo, dto.Observacao);
            _context.MovimentacaoEstoques.Add(movimentacao);

            await SalvarComConcorrencia();

            return movimentacao.IDMovimentacao;
        }

        /// <summary>
        /// Regra única de movimentação: valida o saldo, atualiza o estoque e devolve o registro de histórico.
        /// Reutilizada pelo recebimento de compras.
        /// </summary>
        public static MovimentacaoEstoque AplicarMovimentacao(
            Estoque estoque, TipoMovimentacao tipo, int quantidade, string? motivo, string? observacao)
        {
            var anterior = estoque.QuantidadeAtual;
            var posterior = tipo switch
            {
                TipoMovimentacao.Entrada => anterior + quantidade,
                TipoMovimentacao.Saida when quantidade > anterior =>
                    throw new BusinessException("Não é possível realizar uma saída maior que o saldo atual."),
                TipoMovimentacao.Saida => anterior - quantidade,
                _ => anterior
            };

            estoque.QuantidadeAtual = posterior;
            estoque.DataAtualizacao = DateTime.Now;

            return new MovimentacaoEstoque
            {
                IDProduto = estoque.IDProduto,
                TipoMovimentacao = tipo,
                QuantidadeAnterior = anterior,
                Quantidade = quantidade,
                QuantidadePosterior = posterior,
                Motivo = motivo,
                Observacao = observacao
            };
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

        public async Task<List<MovimentacaoEstoque>> ListarMovimentacao(int? page = null, int? pageSize = null)
        {
            var (skip, take) = Application.Common.PagedQuery.Normalize(page, pageSize);
            var movimentacao = await _context.MovimentacaoEstoques.AsNoTracking().OrderByDescending(x => x.IDMovimentacao).Skip(skip).Take(take).ToListAsync();

            return movimentacao;
        }

        public async Task<MovimentacaoEstoque> ListarMovimentacaoPorID(int IDMovimentacao)
        {
            var movimentacao = await _context.MovimentacaoEstoques.FirstOrDefaultAsync(m => m.IDMovimentacao == IDMovimentacao);

            if (movimentacao is null)
            {
                throw new NotFoundException("Movimentação não encontrada");
            }

            return movimentacao;
        }

        public async Task<int> AtualizarMovimentacaoEstoquePorID(int IDMovimentacao, MovimentacaoEstoqueAtualizarDto movimentacaoEstoqueAtualizarDto)
        {
            var movimentacao = await _context.MovimentacaoEstoques.FirstOrDefaultAsync(m => m.IDMovimentacao == IDMovimentacao);

            if (movimentacao is null)
            {
                throw new NotFoundException("Movimentação não encontrada");
            }

            movimentacao.Motivo = movimentacaoEstoqueAtualizarDto.Motivo;
            movimentacao.Observacao = movimentacaoEstoqueAtualizarDto.Observacao;

            await _context.SaveChangesAsync();

            return movimentacao.IDMovimentacao;
        }


        public async Task<bool> DeletarMovimentacaoEstoque(int IDMovimentacao)
        {
            var movimentacao = await _context.MovimentacaoEstoques.FirstOrDefaultAsync(m => m.IDMovimentacao == IDMovimentacao);

            if (movimentacao is null)
            {
                throw new NotFoundException("Movimentação não encontrada");
            }

            var estoque = await _context.Estoques.FirstOrDefaultAsync(e => e.IDProduto == movimentacao.IDProduto);

            if (estoque is null)
            {
                throw new NotFoundException("Movimentação não encontrada");
            }


            // Excluir uma movimentação desfaz o seu efeito no saldo; nunca pode deixá-lo negativo.
            var saldoAjustado = movimentacao.TipoMovimentacao == TipoMovimentacao.Saida
                ? estoque.QuantidadeAtual + movimentacao.Quantidade
                : estoque.QuantidadeAtual - movimentacao.Quantidade;

            if (saldoAjustado < 0)
            {
                throw new BusinessException("Não é possível excluir a movimentação: o saldo ficaria negativo.");
            }

            estoque.QuantidadeAtual = saldoAjustado;
            estoque.DataAtualizacao = DateTime.Now;

            _context.MovimentacaoEstoques.Remove(movimentacao);
            await SalvarComConcorrencia();

            return true;
        }


    }
}
