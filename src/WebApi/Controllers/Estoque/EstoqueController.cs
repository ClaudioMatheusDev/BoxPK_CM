using Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Service;

namespace WebApi.Controllers.Estoque
{
    [ApiController]
    [Route("api/estoque")]
    [Authorize]
    public class EstoqueController : Controller
    {
        private readonly IEstoqueService _estoqueService;

        public EstoqueController(IEstoqueService estoqueService)
        {
            _estoqueService = estoqueService;
        }

        [HttpPost]
        public async Task<IActionResult> CriarEstoque(EstoqueCriarDto estoqueCriarDto)
        {
            var estoque = await _estoqueService.CriarEstoque(estoqueCriarDto);

            return StatusCode(StatusCodes.Status201Created, new { id = estoque });

        }

        [HttpGet]
        public async Task<IActionResult> ListarEstoque([FromQuery] int? page, [FromQuery] int? pageSize)
        {
            var resultado = await _estoqueService.ListarEstoque(page, pageSize);

            return Ok(resultado);
        }

        [HttpGet("{IDEstoque}")]
        public async Task<IActionResult> ListarEstoquePorID(int IDEstoque)
        {
            var estoque = await _estoqueService.ListarEstoquePorID(IDEstoque);
            return Ok(estoque);
        }

        [HttpDelete("{IDEstoque}")]
        public async Task<IActionResult> DeletarEstoque(int IDEstoque)
        {
            await _estoqueService.DeletarEstoque(IDEstoque);

            return NoContent();
        }

        [HttpPut("{IDEstoque}")]
        public async Task<IActionResult> AtualizarEstoque(int IDEstoque, EstoqueAtualizarDto estoqueAtualizarDto)
        {
            await _estoqueService.AtualizarEstoquePorID(IDEstoque, estoqueAtualizarDto);

            return NoContent();

        }
    }

}
