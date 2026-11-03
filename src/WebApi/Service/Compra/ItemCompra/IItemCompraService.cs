using Application.Dtos;
using Domain.Entities;

namespace WebApi.Service
{
    public interface IItemCompraService
    {
        Task<int> CriarItemCompra(ItemCompraCriarDto itemCompraCriarDto);
        Task<List<ItemCompra>> ListarItemCompra(int? page = null, int? pageSize = null);
        Task<ItemCompra> ListarItemCompraPorID(int IDItemCompra);
        Task<int> AtualizarItemCompraPorID(int IDItemCompra, ItemCompraAtualizarDto itemCompraAtualizarDto);
        Task<bool> DeletarItemCompra(int IDItemCompra);
    }
}
