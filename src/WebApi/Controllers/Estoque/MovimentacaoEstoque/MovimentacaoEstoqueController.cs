using Application.Dtos;
using Microsoft.AspNetCore.Mvc;
using WebApi.Service;

namespace WebApi.Controllers.Estoque
{
    [ApiController]
    [Route("api/movimentacoestoque")]
    public class MovimentacaoEstoqueController : Controller
    {
        private readonly IMovimentacaoEstoque _movimentacaoEstoque;

        public MovimentacaoEstoqueController(IMovimentacaoEstoque movimentacaoEstoque)
        {
            _movimentacaoEstoque = movimentacaoEstoque;
        }

        [HttpPost]
        public async Task<IActionResult> CriarMovimentacao(MovimentacaoEstoqueCriarDto movimentacaoEstoqueCriarDto)
        {
            var movimentacao = await _movimentacaoEstoque.CriarMovimentacaoEstoque(movimentacaoEstoqueCriarDto);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ListarMovimentacoes()
        {
            var movimentacao = await _movimentacaoEstoque.ListarMovimentacao();
            return Ok(movimentacao);
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
            var movimentacao = await _movimentacaoEstoque.DeletarMovimentacaoEstoque(IDMovimentacao);

            return Ok();
        }
    }
}
