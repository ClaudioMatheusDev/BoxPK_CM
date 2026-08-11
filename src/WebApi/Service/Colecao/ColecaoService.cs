using Application.Dtos;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WebApi.Service
{
    public class ColecaoService : IColecaoService
    {

        private readonly AppDbContext _context;

        public ColecaoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> CriarColecao(ColecaoCriarDto colecaoCriarDto)
        {
            var colecao = new Colecao
            {
                Nome = colecaoCriarDto.Nome,
                Sigla = colecaoCriarDto.Sigla,
                IDJogoTCG = colecaoCriarDto.IDJogoTCG,
                DataLancamento = colecaoCriarDto.DataLancamento,
                StatusColecao = StatusColecao.Ativo
            };

            _context.Colecoes.Add(colecao);
            await _context.SaveChangesAsync();

            return colecao.IDColecao;
        }

        public async Task<List<Colecao>> ListarColecao()
        {
            var colecao = await _context.Colecoes.ToListAsync();

            if (colecao is null)
            {
                throw new Exception("Não há colecao cadastrados.");
            }

            return colecao;
        }

        public async Task<Colecao> ListarColecaoPorID(int IDColecao)
        {
            var colecao = await _context.Colecoes.FirstOrDefaultAsync(c => c.IDColecao == IDColecao);

            if (colecao == null)
            {
                throw new Exception("Nenhuma colecao encontrada com esse ID!");
            }


            return colecao;
        }

        public async Task<int> AtualizarColecao(int IDColecao, ColecaoAtualizarDto colecaoAtualizarDto)
        {
            var colecao = await _context.Colecoes.FirstOrDefaultAsync(c => c.IDColecao == IDColecao);

            if (colecao == null)
            {
                throw new Exception("Nenhuma colecao encontrada com esse ID!");
            }

            colecao.Sigla = colecaoAtualizarDto.Sigla;
            colecao.Nome = colecaoAtualizarDto.Nome;
            colecao.IDJogoTCG = colecaoAtualizarDto.IDJogoTCG;
            colecao.DataLancamento = colecaoAtualizarDto.DataLancamento;
            colecao.StatusColecao = colecaoAtualizarDto.StatusColecao;
            colecao.DataAlteracao = DateTime.Now;

            await _context.SaveChangesAsync();

            return colecao.IDColecao;
        }

        public async Task<bool> DeletarColecao(int IDColecao)
        {

            var colecao = await _context.Colecoes.FirstOrDefaultAsync(c => c.IDColecao == IDColecao);

            if (colecao == null)
            {
                throw new Exception("Nenhuma colecao encontrada com esse ID!");
            }

            _context.Colecoes.Remove(colecao);
            await _context.SaveChangesAsync();

            return true;

        }
    }
}
