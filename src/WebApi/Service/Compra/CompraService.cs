using Application.Dtos;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Service
{
    public class CompraService : ICompraService
    {
        private readonly AppDbContext _context;

        public CompraService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> CriarCompra(CompraCriaDto compraCriaDto)
        {
            var compras = new Compra
            {
                IDFornecedor = compraCriaDto.IDFornecedor,
                DataCompra = compraCriaDto.DataCompra,
                ValorTotal = compraCriaDto.ValorTotal,
                Observacao = compraCriaDto.Observacao,
                StatusCompra = StatusCompra.Pendente
            };

            _context.Compras.Add(compras);
            await _context.SaveChangesAsync();

            return compras.IDCompra;

        }
        public Task<List<Compra>> ListarCompras()
        {
            var compras = _context.Compras.ToListAsync();

            return compras;
        }
        public async Task<Compra> ListarCompraPorID(int IDCompra)
        {
            var compras = await _context.Compras.FirstOrDefaultAsync(c => c.IDCompra == IDCompra);

            if (compras is null)
            {
                throw new Exception("Nenhuma compra encontrada com esse ID!");
            }

            return compras;
        }

        public async Task<int> AtualizarCompraPorID(int IDCompra, CompraAtualizarDto compraAtualizarDto)
        {
            var compra = await _context.Compras.FirstOrDefaultAsync(c => c.IDCompra == IDCompra);
            if (compra is null)
            {
                throw new Exception("Nenhuma compra encontrada com esse ID!");
            }

            compra.IDFornecedor = compraAtualizarDto.IDFornecedor;
            compra.DataCompra = compraAtualizarDto.DataCompra;
            compra.ValorTotal = compraAtualizarDto.ValorTotal;
            compra.Observacao = compraAtualizarDto.Observacao;
            compra.StatusCompra = compraAtualizarDto.StatusCompra;

            await _context.SaveChangesAsync();

            return compra.IDCompra;
        }


        public async Task<bool> DeletarCompra(int IDCompra)
        {
            var compra = await _context.Compras.FirstOrDefaultAsync(c => c.IDCompra == IDCompra);

            if (compra is null)
            {
                throw new Exception("Nenhuma compra encontrada com esse ID!");
            }

            _context.Compras.Remove(compra);
            await _context.SaveChangesAsync();

            return true;

        }
    }
}
