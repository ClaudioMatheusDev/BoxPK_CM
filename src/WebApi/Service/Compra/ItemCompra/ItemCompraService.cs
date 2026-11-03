using Application.Common;
using Application.Dtos;
using Application.Exceptions;
using Domain.Entities;
using Domain.Enums;
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

        public async Task<int> CriarItemCompra(ItemCompraCriarDto dto)
        {
            var compra = await ObterCompraPendente(dto.IDCompra);
            await GarantirProduto(dto.IDProduto);

            var itemCompra = new ItemCompra
            {
                IDCompra = dto.IDCompra,
                IDProduto = dto.IDProduto,
                Quantidade = dto.Quantidade,
                ValorUnitario = dto.ValorUnitario,
                ValorTotal = dto.Quantidade * dto.ValorUnitario
            };

            _context.ItensCompras.Add(itemCompra);
            await _context.SaveChangesAsync();
            await RecalcularTotalCompra(compra);

            return itemCompra.IDItemCompra;
        }

        public async Task<List<ItemCompra>> ListarItemCompra(int? page = null, int? pageSize = null)
        {
            var (skip, take) = PagedQuery.Normalize(page, pageSize);
            return await _context.ItensCompras.AsNoTracking().OrderBy(x => x.IDItemCompra).Skip(skip).Take(take).ToListAsync();
        }

        public async Task<ItemCompra> ListarItemCompraPorID(int IDItemCompra)
        {
            var itemCompra = await _context.ItensCompras.AsNoTracking().FirstOrDefaultAsync(i => i.IDItemCompra == IDItemCompra);

            return itemCompra ?? throw new NotFoundException("Nenhum item de compra cadastrado");
        }

        public async Task<int> AtualizarItemCompraPorID(int IDItemCompra, ItemCompraAtualizarDto dto)
        {
            var itemCompra = await _context.ItensCompras.FirstOrDefaultAsync(i => i.IDItemCompra == IDItemCompra)
                ?? throw new NotFoundException("Nenhum item de compra cadastrado");

            var compraAntiga = await ObterCompraPendente(itemCompra.IDCompra);
            var compraNova = dto.IDCompra == itemCompra.IDCompra ? compraAntiga : await ObterCompraPendente(dto.IDCompra);
            await GarantirProduto(dto.IDProduto);

            itemCompra.IDCompra = dto.IDCompra;
            itemCompra.IDProduto = dto.IDProduto;
            itemCompra.Quantidade = dto.Quantidade;
            itemCompra.ValorUnitario = dto.ValorUnitario;
            itemCompra.ValorTotal = dto.Quantidade * dto.ValorUnitario;

            await _context.SaveChangesAsync();

            await RecalcularTotalCompra(compraNova);
            if (compraAntiga.IDCompra != compraNova.IDCompra)
            {
                await RecalcularTotalCompra(compraAntiga);
            }

            return itemCompra.IDCompra;
        }

        public async Task<bool> DeletarItemCompra(int IDItemCompra)
        {
            var itemCompra = await _context.ItensCompras.FirstOrDefaultAsync(i => i.IDItemCompra == IDItemCompra)
                ?? throw new NotFoundException("Nenhum item de compra cadastrado");

            var compra = await ObterCompraPendente(itemCompra.IDCompra);

            _context.ItensCompras.Remove(itemCompra);
            await _context.SaveChangesAsync();
            await RecalcularTotalCompra(compra);

            return true;
        }

        /// <summary>Itens só podem ser alterados enquanto a compra está pendente (depois ela já gerou estoque).</summary>
        private async Task<Compra> ObterCompraPendente(int idCompra)
        {
            var compra = await _context.Compras.FirstOrDefaultAsync(c => c.IDCompra == idCompra)
                ?? throw new NotFoundException("Nenhuma compra encontrada com esse ID!");

            if (compra.StatusCompra != StatusCompra.Pendente)
            {
                throw new BusinessException("Somente compras pendentes podem ter itens alterados.");
            }

            return compra;
        }

        private async Task GarantirProduto(int idProduto)
        {
            if (!await _context.Produtos.AnyAsync(p => p.IDProduto == idProduto))
            {
                throw new NotFoundException("Produto não encontrado.");
            }
        }

        /// <summary>Recalcula o total a partir dos itens já gravados e persiste a compra.</summary>
        private async Task RecalcularTotalCompra(Compra compra)
        {
            compra.ValorTotal = await _context.ItensCompras
                .Where(i => i.IDCompra == compra.IDCompra)
                .SumAsync(i => i.ValorTotal);

            await _context.SaveChangesAsync();
        }
    }
}
