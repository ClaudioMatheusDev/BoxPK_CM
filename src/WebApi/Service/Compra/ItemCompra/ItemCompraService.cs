using Application.Dtos;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace WebApi.Service
{
    public class ItemCompraService : IItemCompraService
    {
        private readonly AppDbContext _context;

        public ItemCompraService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> CriarItemCompra(ItemCompraCriarDto itemCompraCriarDto)
        {
            var itemCompra = new ItemCompra
            {
                IDCompra = itemCompraCriarDto.IDCompra,
                IDProduto = itemCompraCriarDto.IDProduto,
                Quantidade = itemCompraCriarDto.Quantidade,
                ValorUnitario = itemCompraCriarDto.ValorUnitario,
                ValorTotal = itemCompraCriarDto.ValorTotal
            };

            _context.ItensCompras.Add(itemCompra);
            await _context.SaveChangesAsync();

            return itemCompra.IDItemCompra;
        }
        public async Task<List<ItemCompra>> ListarItemCompra()
        {
            var itemCompra = await _context.ItensCompras.ToListAsync();

            return itemCompra;
        }

        public async Task<ItemCompra> ListarItemCompraPorID(int IDItemCompra)
        {
            var itemCompra = await _context.ItensCompras.FirstOrDefaultAsync(i => i.IDItemCompra == IDItemCompra);

            if (itemCompra == null)
            {
                throw new Exception("Nenhum item de compra cadastrado");
            }

            return itemCompra;
        }

        public async Task<int> AtualizarItemCompraEstoquePorID(int IDItemCompra, ItemCompraAtualizarDto itemCompraAtualizarDto)
        {
            var itemCompra = await _context.ItensCompras.FirstOrDefaultAsync(i => i.IDItemCompra == IDItemCompra);

            if (itemCompra == null)
            {
                throw new Exception("Nenhum item de compra cadastrado");
            }

            itemCompra.IDCompra = itemCompraAtualizarDto.IDCompra;
            itemCompra.IDProduto = itemCompraAtualizarDto.IDProduto;
            itemCompra.Quantidade = itemCompraAtualizarDto.Quantidade;
            itemCompra.ValorUnitario = itemCompraAtualizarDto.ValorUnitario;
            itemCompra.ValorTotal = itemCompraAtualizarDto.ValorTotal;

            await _context.SaveChangesAsync();

            return itemCompra.IDCompra;
        }


        public async Task<bool> DeletarItemCompra(int IDItemCompra)
        {
            var itemCompra = await _context.ItensCompras.FirstOrDefaultAsync(i => i.IDItemCompra == IDItemCompra);

            if (itemCompra == null)
            {
                throw new Exception("Nenhum item de compra cadastrado");
            }

            _context.ItensCompras.Remove(itemCompra);
            await _context.SaveChangesAsync();

            return true;
        }

    }
}
