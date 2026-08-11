using Application.Dtos;
using Domain.Entities;

namespace WebApi.Service
{
    public interface IItemCompraService
    {
        Task<int> CriarItemCompra(ItemCompraCriarDto itemCompraCriarDto);
        Task<List<ItemCompra>> ListarItemCompra();
        Task<ItemCompra> ListarItemCompraPorID(int IDItemCompra);
        Task<int> AtualizarItemCompraEstoquePorID(int IDItemCompra, ItemCompraAtualizarDto itemCompraAtualizarDto);
        Task<bool> DeletarItemCompra(int IDItemCompra);
    }
}
