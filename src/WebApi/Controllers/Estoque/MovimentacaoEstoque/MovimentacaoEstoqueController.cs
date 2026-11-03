using Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Service;

namespace WebApi.Controllers.Estoque
{
    [ApiController]
    [Route("api/movimentacoestoque")]
    [Authorize]
    public class MovimentacaoEstoqueController : Controller
    {
        private readonly IMovimentacaoEstoqueService _movimentacaoEstoque;

        public MovimentacaoEstoqueController(IMovimentacaoEstoqueService movimentacaoEstoque)
        {
            _movimentacaoEstoque = movimentacaoEstoque;
        }

        [HttpPost]
        public async Task<IActionResult> CriarMovimentacao(MovimentacaoEstoqueCriarDto movimentacaoEstoqueCriarDto)
        {
            var movimentacao = await _movimentacaoEstoque.CriarMovimentacaoEstoque(movimentacaoEstoqueCriarDto);

            return StatusCode(StatusCodes.Status201Created, new { id = movimentacao });
        }

        [HttpGet]
        public async Task<IActionResult> ListarMovimentacoes([FromQuery] int? page, [FromQuery] int? pageSize)
        {
            var resultado = await _movimentacaoEstoque.ListarMovimentacao(page, pageSize);

            return Ok(resultado);
        }

        [HttpGet("{IDMovimentacao}")]
        public async Task<IActionResult> ListarMovimentacaoPorID(int IDMovimentacao)
        {
            var movimentacao = await _movimentacaoEstoque.ListarMovimentacaoPorID(IDMovimentacao);
            return Ok(movimentacao);
        }

        [HttpPut("{IDMovimentacao}")]
        public async Task<IActionResult> AtualizarMovimentacao(int IDMovimentacao, MovimentacaoEstoqueAtualizarDto movimentacaoEstoqueAtualizarDto)
        {
            var movimentacao = await _movimentacaoEstoque.AtualizarMovimentacaoEstoquePorID(IDMovimentacao, movimentacaoEstoqueAtualizarDto);

            return Ok(movimentacao);
        }

        [HttpDelete("{IDMovimentacao}")]
        public async Task<IActionResult> DeletarMovimentacao(int IDMovimentacao)
        {
            await _movimentacaoEstoque.DeletarMovimentacaoEstoque(IDMovimentacao);

            return NoContent();
        }
    }
}
