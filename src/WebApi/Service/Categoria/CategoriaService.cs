using Application.Dtos;
using Application.Dtos.Categoria;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;


namespace WebApi.Service
{
    public class CategoriaService : ICategoriaService
    {
        private readonly AppDbContext _context;

        public CategoriaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> CriarCategoria(CategoriaCriarDto categoriaCriarDto)
        {
            var categoria = new Categoria
            {
                Nome = categoriaCriarDto.Nome,
                Descricao = categoriaCriarDto.Descricao,
                StatusCategoria = StatusCategoria.Ativo
            };

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            return categoria.IDCategoria;
        }

        public async Task<List<Categoria>> ListarCategorias()
        {
            var categoria = await _context.Categorias.ToListAsync();

            if (categoria is null)
            {
                throw new Exception("Não há categoria cadastrados.");
            }

            return categoria;
        }

        public async Task<Categoria> ListarCategoriaPorID(int IDCategoria)
        {
            var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.IDCategoria == IDCategoria);


            if (categoria is null)
            {
                throw new Exception("Não há categoria cadastrados.");
            }

            return categoria;

        }

        public async Task<int> AtualizarCategoria(int IDCategoria, CategoriaAtualizarDto categoriaAtualizarDto)
        {
            var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.IDCategoria == IDCategoria);

            if (categoria is null)
            {
                throw new Exception("Nenhuma categoria encontra nessa ID");
            }

            categoria.Nome = categoriaAtualizarDto.Nome;
            categoria.Descricao = categoriaAtualizarDto.Descricao;
            categoria.StatusCategoria = categoriaAtualizarDto.StatusCategoria;
            categoria.DataAlteracao = DateTime.Now.AddHours(-3);

            await _context.SaveChangesAsync();

            return categoria.IDCategoria;

        }

        public async Task<bool> DeletarCategoria(int IDCategoria)
        {
            var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.IDCategoria == IDCategoria);


            if (categoria is null)
            {
                throw new Exception("Nenhuma categoria encontra nessa ID");
            }

            _context.Remove(categoria);
            await _context.SaveChangesAsync();

            return true;

        }





    }
}
