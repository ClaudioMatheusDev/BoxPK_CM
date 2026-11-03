using Application.Dtos;
using Domain.Entities;

namespace WebApi.Service
{
    public interface ICompraService
    {
        Task<int> CriarCompra(CompraCriarDto compraCriaDto);
        Task<List<Compra>> ListarCompras(int? page = null, int? pageSize = null);
        Task<Compra> ListarCompraPorID(int IDCompra);
        Task<int> AtualizarCompraPorID(int IDCompra, CompraAtualizarDto compraAtualizarDto);
        Task<bool> DeletarCompra(int IDCompra);
    }
}
