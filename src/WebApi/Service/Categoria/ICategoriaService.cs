using Application.Dtos;
using Application.Dtos.Categoria;
using Domain.Entities;

namespace WebApi.Service
{
    public interface ICategoriaService
    {
        Task<int> CriarCategoria(CategoriaCriarDto categoriaCriarDto);
        Task<List<Categoria>> ListarCategorias();
        Task<Categoria> ListarCategoriaPorID(int IDCategoria);
        Task<int> AtualizarCategoria(int IDCategoria, CategoriaAtualizarDto categoriaAtualizarDto);
        Task<bool> DeletarCategoria(int IDCategorai);
    }
}
