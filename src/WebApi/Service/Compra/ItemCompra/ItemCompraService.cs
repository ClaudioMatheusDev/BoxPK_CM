using Application.Dtos;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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

            var valorTotalCompra = itemCompraCriarDto.Quantidade * itemCompraCriarDto.ValorUnitario;

            var itemCompra = new ItemCompra
            {
                IDCompra = itemCompraCriarDto.IDCompra,
                IDProduto = itemCompraCriarDto.IDProduto,
                Quantidade = itemCompraCriarDto.Quantidade,
                ValorUnitario = itemCompraCriarDto.ValorUnitario,
                ValorTotal = valorTotalCompra
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
            var valorTotalCompra = itemCompraAtualizarDto.Quantidade * itemCompraAtualizarDto.ValorUnitario;

            if (itemCompra == null)
            {
                throw new Exception("Nenhum item de compra cadastrado");
            }

            itemCompra.IDCompra = itemCompraAtualizarDto.IDCompra;
            itemCompra.IDProduto = itemCompraAtualizarDto.IDProduto;
            itemCompra.Quantidade = itemCompraAtualizarDto.Quantidade;
            itemCompra.ValorUnitario = itemCompraAtualizarDto.ValorUnitario;
            itemCompra.ValorTotal = valorTotalCompra;

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
