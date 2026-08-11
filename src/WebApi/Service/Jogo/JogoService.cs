using Application.Dtos;
using Application.Dtos.Jogo;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Service
{
    public class JogoService : IJogoService
    {

        private readonly AppDbContext _context;


        public JogoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> CriarJogo(JogoCriarDto jogoCriarDto)
        {
            var jogo = new Jogo
            {
                Nome = jogoCriarDto.Nome,
                Fabricante = jogoCriarDto.Fabricante,
                StatusJogo = StatusJogo.Ativo
            };

            _context.Jogos.Add(jogo);
            await _context.SaveChangesAsync();

            return jogo.IDJogoTCG;
        }

        public async Task<List<Jogo>> ListarJogos()
        {
            var jogo = await _context.Jogos.ToListAsync();

            return jogo;
        }

        public async Task<Jogo> ListarJogoPorID(int IDJogosTCG)
        {
            var jogo = await _context.Jogos.FirstOrDefaultAsync(j => j.IDJogoTCG == IDJogosTCG);

            if (jogo is null)
            {
                throw new Exception("Não há jogo cadastrados.");
            }

            return jogo;
        }

        public async Task<int> AtualizarJogoPorID(int IDJogosTCG, JogoAtualizarDto jogoAtualizarDto)
        {
            var jogo = await _context.Jogos.FirstOrDefaultAsync(j => j.IDJogoTCG == IDJogosTCG);

            if (jogo is null)
            {
                throw new Exception("Jogo não encontrado");
            }

            jogo.Nome = jogoAtualizarDto.Nome;
            jogo.Fabricante = jogoAtualizarDto.Fabricante;
            jogo.StatusJogo = jogoAtualizarDto.StatusJogo;
            jogo.DataAtualizacao = DateTime.Now;

            await _context.SaveChangesAsync();

            return jogo.IDJogoTCG;
        }


        public async Task<bool> DeletarJogo(int IDJogosTCG)
        {
            var jogo = await _context.Jogos.FirstOrDefaultAsync(j => j.IDJogoTCG == IDJogosTCG);

            if (jogo is null)
            {
                throw new Exception("Jogo não encontrado");
            }

            _context.Remove(jogo);
            await _context.SaveChangesAsync();
            return true;

        }

    }
}
