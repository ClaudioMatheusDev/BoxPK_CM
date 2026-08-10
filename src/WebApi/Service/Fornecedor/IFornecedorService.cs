using Application.Dtos;
using Domain.Entities;


namespace WebApi.Service
{
    public interface IFornecedorService
    {
        Task<int> CriarFornecedor(FornecedorCriarDto FornecedorCriarDto);
        Task<List<Fornecedor>> ListarFornecedores();
        Task<Fornecedor> ListarFornecedorPorID(int IDFornecedor);
        Task<int> AtualizarFornecedor(int IDFornecedor, FornecedorAtualizarDto fornecedorAtualizarDto);
        Task<bool> DeletarFornecedor(int IDFornecedor);
    }
}
