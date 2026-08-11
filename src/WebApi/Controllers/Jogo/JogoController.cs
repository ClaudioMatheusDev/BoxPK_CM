using Application.Dtos;
using Application.Dtos.Jogo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Service;

namespace WebApi.Controllers.Jogo
{
    [ApiController]
    [Route("api/jogo")]
    [Authorize]
    public class JogoController : Controller
    {

        private readonly IJogoService _fornecedorService;

        public JogoController(IJogoService jogoService)
        {
            _fornecedorService = jogoService;
        }

        [HttpPost]
        public async Task<IActionResult> CriarJogo(JogoCriarDto jogoCriarDto)
        {
            var jogo = await _fornecedorService.CriarJogo(jogoCriarDto);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ListarJogos()
        {
            var jogo = await _fornecedorService.ListarJogos();

            return Ok(jogo);
        }

        [HttpGet("{IDJogoTCG}")]
        public async Task<IActionResult> ListarJogoPorID(int IDJogoTCG)
        {
            var jogo = await _fornecedorService.ListarJogoPorID(IDJogoTCG);

            return Ok(jogo);
        }

        [HttpPut("{IDJogoTCG}")]
        public async Task<IActionResult> AtualizarJogo(int IDJogoTCG, JogoAtualizarDto jogoAtualizarDto)
        {
            var jogo = await _fornecedorService.AtualizarJogoPorID(IDJogoTCG, jogoAtualizarDto);


            if (jogo == 0)
            {
                return NotFound();
            }
            return Ok();
        }

        [HttpDelete("{IDJogoTCG}")]
        public async Task<IActionResult> DeletarJogo(int IDJogoTCG)
        {
            var jogo = await _fornecedorService.DeletarJogo(IDJogoTCG);
            if (!jogo)
            {
                return NotFound();
            }

            return Ok();

        }
    }
}
