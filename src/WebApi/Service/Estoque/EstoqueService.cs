using Application.Dtos;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Service
{
    public class EstoqueService : IEstoqueService
    {
        private readonly AppDbContext _context;

        public EstoqueService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> CriarEstoque(EstoqueCriarDto estoqueCriarDto)
        {
            var estoque = new Estoque
            {
                IDProduto = estoqueCriarDto.IDProduto,
                QuantidadeAtual = estoqueCriarDto.QuantidadeAtual,
                QuantidadeMinima = estoqueCriarDto.QuantidadeMinima,
                QuantidadeMaxima = estoqueCriarDto.QuantidadeMaxima
            };

            _context.Estoques.Add(estoque);
            await _context.SaveChangesAsync();

            return estoque.IDEstoque;
        }

        public async Task<List<Estoque>> ListarEstoque()
        {
            var estoque = await _context.Estoques.ToListAsync();

            return estoque;
        }

        public async Task<Estoque> ListarEstoquePorID(int IDEstoque)
        {
            var estoque = await _context.Estoques.FirstOrDefaultAsync(e => e.IDEstoque == IDEstoque);

            if (estoque is null)
            {
                throw new Exception("Não foi encontrando nenhum estoque com esse ID!");
            }

            return estoque;
        }

        public async Task<int> AtualizarEstoquePorID(int IDEstoque, EstoqueAtualizarDto estoqueAtualizarDto)
        {

            var estoque = await _context.Estoques.FirstOrDefaultAsync(e => e.IDEstoque == IDEstoque);

            if (estoque is null)
            {
                throw new Exception("Não foi encontrando nenhum estoque com esse ID!");
            }

            estoque.IDProduto = estoqueAtualizarDto.IDProduto;
            estoque.QuantidadeMinima = estoqueAtualizarDto.QuantidadeMinima;
            estoque.QuantidadeMaxima = estoqueAtualizarDto.QuantidadeMaxima;
            estoque.QuantidadeAtual = estoqueAtualizarDto.QuantidadeAtual;
            estoque.DataAtualizacao = DateTime.Now;

            await _context.SaveChangesAsync();

            return estoque.IDProduto;

        }

        public async Task<bool> DeletarEstoque(int IDEstoque)
        {
            var estoque = await _context.Estoques.FirstOrDefaultAsync(e => e.IDEstoque == IDEstoque);

            if (estoque is null)
            {
                throw new Exception("Não foi encontrando nenhum estoque com esse ID!");
            }

            _context.Estoques.Remove(estoque);
            await _context.SaveChangesAsync();

            return true;

        }

    }
}
