using Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Service;

namespace WebApi.Controllers.Colecao
{
    [ApiController]
    [Route("api/colecao")]
    [Authorize]
    public class ColecaoController : Controller
    {

        private readonly IColecaoService _colecaoService;


        public ColecaoController(IColecaoService colecaoService)
        {
            _colecaoService = colecaoService;
        }

        [HttpPost]
        public async Task<IActionResult> CriarColecao(ColecaoCriarDto colecaoCriarDto)
        {
            var colecao = await _colecaoService.CriarColecao(colecaoCriarDto);

            return StatusCode(StatusCodes.Status201Created, new { id = colecao });
        }

        [HttpGet]
        public async Task<IActionResult> ListarColecao([FromQuery] int? page, [FromQuery] int? pageSize) 
        {
            var resultado = await _colecaoService.ListarColecao(page, pageSize);

            return Ok(resultado);
        }

        [HttpGet("{IDColecao}")]
        public async Task<IActionResult> ListarColecaoPorID(int IDColecao)
        {
            var colecao = await _colecaoService.ListarColecaoPorID(IDColecao);

            return Ok(colecao);
        }

        [HttpPut("{IDColecao}")]
        public async Task<IActionResult> AtualizarColecao(int IDColecao, ColecaoAtualizarDto colecaoAtualizarDto) 
        {
            await _colecaoService.AtualizarColecao(IDColecao, colecaoAtualizarDto);

            return NoContent();
        }

        [HttpDelete("{IDColecao}")]
        public async Task<IActionResult> DeletarColecao(int IDColecao)
        {
            await _colecaoService.DeletarColecao(IDColecao);

            return NoContent();
        }
    }
}
