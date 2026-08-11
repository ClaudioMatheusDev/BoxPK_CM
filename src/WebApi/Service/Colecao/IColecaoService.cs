using Application.Dtos;
using Domain.Entities;

namespace WebApi.Service
{
    public interface IColecaoService
    {
        Task<int> CriarColecao(ColecaoCriarDto colecaoCriarDto);
        Task<List<Colecao>> ListarColecao();
        Task<Colecao> ListarColecaoPorID(int IDColecao);
        Task<int> AtualizarColecao(int IDColecao, ColecaoAtualizarDto colecaoAtualizarDto);
        Task<bool> DeletarColecao(int IDColecao);
    }
}
