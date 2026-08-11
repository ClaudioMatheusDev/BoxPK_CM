using Application.Dtos;
using Application.Dtos.Produto;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Runtime.CompilerServices;

namespace WebApi.Service
{
    public class ProdutoService : IProdutoService
    {
        private readonly AppDbContext _context;

        public ProdutoService(AppDbContext context)
        {
            _context = context;
        }


        public async Task<int> CriarProduto(ProdutoCriarDto produtoCriarDto)
        {
            var produto = new Produto
            {
                Nome = produtoCriarDto.Nome,
                Descricao = produtoCriarDto.Descricao,
                PrecoCusto = produtoCriarDto.PrecoCusto,
                PrecoVenda = produtoCriarDto.PrecoVenda,
                IDCategoria = produtoCriarDto.IDCategoria,
                IDJogoTCG = produtoCriarDto.IDJogoTCG,
                IDColecao = produtoCriarDto.IDColecao,
                StatusProduto = StatusProduto.Ativo
            };

            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            return produto.IDProduto;
        }

        public async Task<List<Produto>> ListarProdutos()
        {
            var produto = await _context.Produtos.ToListAsync();


            return produto;
        }

        public async Task<Produto> ListarProdutoPorID(int IDProduto)
        {
            var produto = await _context.Produtos.FirstOrDefaultAsync(p => p.IDProduto == IDProduto);

            if (produto is null)
            {
                throw new Exception("Não foi encontrado o produto com o ID digitado!");
            }
            return produto;
        }

        public async Task<int> AtualizarProduto(int IDProduto, ProdutoAtualizarDto produtoAtualizarDto)
        {
            var produto = await _context.Produtos.FirstOrDefaultAsync(p => p.IDProduto == IDProduto);

            if (produto is null)
            {
                throw new Exception("Não foi encontrado o produto com o ID digitado!");
            }

            produto.Nome = produtoAtualizarDto.Nome;
            produto.Descricao = produtoAtualizarDto.Descricao;
            produto.PrecoCusto = produtoAtualizarDto.PrecoCusto;
            produto.PrecoVenda = produtoAtualizarDto.PrecoVenda;
            produto.IDCategoria = produtoAtualizarDto.IDCategoria;
            produto.IDJogoTCG = produtoAtualizarDto.IDJogoTCG;
            produto.IDColecao = produtoAtualizarDto.IDColecao;
            produto.StatusProduto = produtoAtualizarDto.StatusProduto;
            produto.DataAlteracao = DateTime.Now;

            await _context.SaveChangesAsync();

            return produto.IDProduto;
        }

        public async Task<bool> DeletarProduto(int IDProduto)
        {
            var produto = await _context.Produtos.FirstOrDefaultAsync(p => p.IDProduto == IDProduto);

            if (produto is null)
            {
                throw new Exception("Não foi encontrado o produto com o ID digitado!");
            }

            _context.Produtos.Remove(produto);

            await _context.SaveChangesAsync();

            return true;

        }
    }
}

