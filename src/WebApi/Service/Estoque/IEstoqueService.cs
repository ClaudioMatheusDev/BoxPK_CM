using Application.Dtos;
using Domain.Entities;

namespace WebApi.Service
{
    public interface IEstoqueService
    {
        Task<int> CriarEstoque(EstoqueCriarDto estoqueCriarDto);
        Task<List<Estoque>> ListarEstoque();
        Task<Estoque> ListarEstoquePorID(int IDEstoque);
        Task<int> AtualizarEstoquePorID(int IDEstoque, EstoqueAtualizarDto estoqueAtualizarDto);
        Task<bool> DeletarEstoque(int IDEstoque);
    }
}
