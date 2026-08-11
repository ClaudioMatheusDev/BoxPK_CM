using Application.Dtos;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http.HttpResults;
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

        public async Task<int> CriarMovimentacaoEstoque(MovimentacaoEstoqueCriarDto movimentacaoEstoqueCriarDto)
        {
            var movimentacao = new MovimentacaoEstoque
            {
                IDProduto = movimentacaoEstoqueCriarDto.IDProduto,
                TipoMovimentacao = movimentacaoEstoqueCriarDto.TipoMovimentacao,
                QuantidadeAnterior = movimentacaoEstoqueCriarDto.QuantidadeAnterior,
                Quantidade = movimentacaoEstoqueCriarDto.Quantidade,
                QuantidadePosterior = movimentacaoEstoqueCriarDto.QuantidadePosterior,
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

            movimentacao.IDProduto = movimentacaoEstoqueAtualizarDto.IDProduto;
            movimentacao.TipoMovimentacao = movimentacaoEstoqueAtualizarDto.TipoMovimentacao;
            movimentacao.QuantidadeAnterior = movimentacaoEstoqueAtualizarDto.QuantidadeAnterior;
            movimentacao.Quantidade = movimentacaoEstoqueAtualizarDto.Quantidade;
            movimentacao.QuantidadePosterior = movimentacaoEstoqueAtualizarDto.QuantidadePosterior;
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

            _context.MovimentacaoEstoques.Remove(movimentacao);
            await _context.SaveChangesAsync();

            return true;
        }


    }
}
