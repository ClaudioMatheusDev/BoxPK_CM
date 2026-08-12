using Application.Dtos;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace WebApi.Service
{
    public class ItemCompraService : IItemCompraService
    {
        private readonly AppDbContext _context;
        public ItemCompraService(AppDbContext context)
        {
            _context = context;
        }

         private async Task RecalcularTotalCompra(int idCompra)
        {
            var compra = await _context.Compras.FirstOrDefaultAsync(c => c.IDCompra == idCompra);
            if (compra == null) return;

            var itens = await _context.ItensCompras
                .Where(i => i.IDCompra == idCompra)
                .ToListAsync();

            var totalItens = itens.Sum(i => i.ValorTotal);

            compra.ValorTotal = totalItens;
        }

        public async Task<int> CriarItemCompra(ItemCompraCriarDto itemCompraCriarDto)
        {
            var compra = await _context.Compras.FirstOrDefaultAsync(c => c.IDCompra == itemCompraCriarDto.IDCompra);
            if (compra == null)
            {
                throw new Exception("Nenhuma compra cadastrado");
            }

            var valorTotalItem = itemCompraCriarDto.Quantidade * itemCompraCriarDto.ValorUnitario;

            var itemCompra = new ItemCompra
            {
                IDCompra = itemCompraCriarDto.IDCompra,
                IDProduto = itemCompraCriarDto.IDProduto,
                Quantidade = itemCompraCriarDto.Quantidade,
                ValorUnitario = itemCompraCriarDto.ValorUnitario,
                ValorTotal = valorTotalItem
            };

            _context.ItensCompras.Add(itemCompra);
            await _context.SaveChangesAsync();

            await RecalcularTotalCompra(itemCompraCriarDto.IDCompra);
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

            var idCompraAntiga = itemCompra.IDCompra;
            var valorTotalItem = itemCompraAtualizarDto.Quantidade * itemCompraAtualizarDto.ValorUnitario;

            itemCompra.IDCompra = itemCompraAtualizarDto.IDCompra;
            itemCompra.IDProduto = itemCompraAtualizarDto.IDProduto;
            itemCompra.Quantidade = itemCompraAtualizarDto.Quantidade;
            itemCompra.ValorUnitario = itemCompraAtualizarDto.ValorUnitario;
            itemCompra.ValorTotal = valorTotalItem;

            await _context.SaveChangesAsync();

            await RecalcularTotalCompra(itemCompraAtualizarDto.IDCompra);
            if (idCompraAntiga != itemCompraAtualizarDto.IDCompra)
            {
                await RecalcularTotalCompra(idCompraAntiga);
            }
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

            var idCompra = itemCompra.IDCompra;

            _context.ItensCompras.Remove(itemCompra);
            await _context.SaveChangesAsync();

            await RecalcularTotalCompra(idCompra);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}