using Application.Dtos;
using Application.Dtos.Produto;
using Domain.Entities;

namespace WebApi.Service
{
    public interface IProdutoService
    {
        Task<int> CriarProduto(ProdutoCriarDto produtoCriarDto);
        Task<List<Produto>> ListarProdutos();
        Task<Produto> ListarProdutoPorID(int IDProduto);
        Task<int> AtualizarProduto(int IDProduto, ProdutoAtualizarDto produtoAtualizarDto);
        Task<bool> DeletarProduto(int IDProduto);
    }
}
