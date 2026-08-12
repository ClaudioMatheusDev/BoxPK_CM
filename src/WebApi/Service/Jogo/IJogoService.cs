using Application.Dtos;
using Application.Dtos.Jogo;
using Domain.Entities;

namespace WebApi.Service
{
    public interface IJogoService
    {
        Task<int> CriarJogo(JogoCriarDto jogoCriarDto);
        Task<List<Jogo>> ListarJogos();
        Task<Jogo> ListarJogoPorID(int IDJogoTCG);
        Task<int> AtualizarJogoPorID(int IDJogoTCG, JogoAtualizarDto jogoAtualizarDto);
        Task<bool> DeletarJogo(int IDJogoTCG);
    }
}
