using Application.Dtos;
using Domain.Entities;

namespace WebApi.Service
{
    public interface IMovimentacaoEstoque
    {
        Task<int> CriarMovimentacaoEstoque(MovimentacaoEstoqueCriarDto movimentacaoEstoqueCriarDto);
        Task<List<MovimentacaoEstoque>> ListarMovimentacao();
        Task<MovimentacaoEstoque> ListarMovimentacaoPorID(int IDMovimentacao);
        Task<int> AtualizarMovimentacaoEstoquePorID(int IDMovimentacao, MovimentacaoEstoqueAtualizarDto movimentacaoEstoqueAtualizarDto);
        Task<bool> DeletarMovimentacaoEstoque(int IDMovimentacao);
    }
}
