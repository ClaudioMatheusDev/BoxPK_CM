using Application.Dtos;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Service
{
    public class MovimentacaoEstoqueService : IMovimentacaoEstoque
    {
        private readonly AppDbContext _context;

        public MovimentacaoEstoqueService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> CriarMovimentacaoEstoque(
     MovimentacaoEstoqueCriarDto movimentacaoEstoqueCriarDto)
        {
            var estoque = await _context.Estoques.FirstOrDefaultAsync(e => e.IDProduto == movimentacaoEstoqueCriarDto.IDProduto);

            if (estoque is null)
            {
                throw new Exception("Estoque não encontrado.");
            }

            if (movimentacaoEstoqueCriarDto.Quantidade <= 0)
            {
                throw new Exception("A quantidade da movimentação deve ser maior que zero.");
            }

            var quantidadeAnterior = estoque.QuantidadeAtual;
            var quantidadePosterior = quantidadeAnterior;

            if (movimentacaoEstoqueCriarDto.TipoMovimentacao == TipoMovimentacao.Entrada)
            {
                quantidadePosterior = quantidadeAnterior + movimentacaoEstoqueCriarDto.Quantidade;
            }

            if (movimentacaoEstoqueCriarDto.TipoMovimentacao == TipoMovimentacao.Saida)
            {
                if (movimentacaoEstoqueCriarDto.Quantidade > quantidadeAnterior)
                {
                    throw new Exception("Não é possível realizar uma saída maior que o saldo atual.");
                }

                quantidadePosterior = quantidadeAnterior - movimentacaoEstoqueCriarDto.Quantidade;
            }

            estoque.QuantidadeAtual = quantidadePosterior;

            var movimentacao = new MovimentacaoEstoque
            {
                IDProduto = movimentacaoEstoqueCriarDto.IDProduto,
                TipoMovimentacao = movimentacaoEstoqueCriarDto.TipoMovimentacao,
                QuantidadeAnterior = quantidadeAnterior,
                Quantidade = movimentacaoEstoqueCriarDto.Quantidade,
                QuantidadePosterior = quantidadePosterior,
                Motivo = movimentacaoEstoqueCriarDto.Motivo,
                Observacao = movimentacaoEstoqueCriarDto.Observacao
            };

            _context.MovimentacaoEstoques.Add(movimentacao);

            await _context.SaveChangesAsync();

            return movimentacao.IDMovimentacao;
        }

        public async Task<List<MovimentacaoEstoque>> ListarMovimentacao()
        {
            var movimentacao = await _context.MovimentacaoEstoques.ToListAsync();

            return movimentacao;
        }

        public async Task<MovimentacaoEstoque> ListarMovimentacaoPorID(int IDMovimentacao)
        {
            var movimentacao = await _context.MovimentacaoEstoques.FirstOrDefaultAsync(m => m.IDMovimentacao == IDMovimentacao);

            if (movimentacao is null)
            {
                throw new Exception("Movimentação não encontrada");
            }

            return movimentacao;
        }

        public async Task<int> AtualizarMovimentacaoEstoquePorID(int IDMovimentacao, MovimentacaoEstoqueAtualizarDto movimentacaoEstoqueAtualizarDto)
        {
            var movimentacao = await _context.MovimentacaoEstoques.FirstOrDefaultAsync(m => m.IDMovimentacao == IDMovimentacao);

            if (movimentacao is null)
            {
                throw new Exception("Movimentação não encontrada");
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
                throw new Exception("Movimentação não encontrada");
            }

            var estoque = await _context.Estoques.FirstOrDefaultAsync(e => e.IDProduto == movimentacao.IDProduto);

            if (estoque is null)
            {
                throw new Exception("Movimentação não encontrada");
            }


            var quantidadeAnterior = estoque.QuantidadeAtual;

            var quantidadeAjustadaSaida = quantidadeAnterior + movimentacao.Quantidade;
            var quantidadeAjustadaEntrada = quantidadeAnterior - movimentacao.Quantidade;

            if (movimentacao.TipoMovimentacao == TipoMovimentacao.Saida)
            {

                estoque.QuantidadeAtual = quantidadeAjustadaSaida;
            }

            if (movimentacao.TipoMovimentacao == TipoMovimentacao.Entrada)
            {
                estoque.QuantidadeAtual = quantidadeAjustadaEntrada;
            }

            _context.MovimentacaoEstoques.Remove(movimentacao);
            await _context.SaveChangesAsync();

            return true;
        }


    }
}
