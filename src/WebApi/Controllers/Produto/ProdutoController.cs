using Application.Dtos;
using Application.Dtos.Produto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Service;

namespace WebApi.Controllers.Produto
{
    [ApiController]
    [Route("api/produto")]
    [Authorize]
    public class ProdutoController : Controller
    {

        private readonly IProdutoService _produtoService;

        public ProdutoController(IProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        [HttpPost]
        public async Task<IActionResult> CriarProduto(ProdutoCriarDto produtoCriarDto)
        {
            var produto = await _produtoService.CriarProduto(produtoCriarDto);

            return StatusCode(StatusCodes.Status201Created, new { id = produto });
        }

        [HttpGet]
        public async Task<IActionResult> ListarProdutos([FromQuery] int? page, [FromQuery] int? pageSize)
        {
            var resultado = await _produtoService.ListarProdutos(page, pageSize);

            return Ok(resultado);
        }

        [HttpGet("{IDproduto}")]
        public async Task<IActionResult> ListarProdutosPorID(int IDproduto)
        {
            var produto = await _produtoService.ListarProdutoPorID(IDproduto);

            return Ok(produto);
        }

        [HttpPut("{IDProduto}")]

        public async Task<IActionResult> AtualizarProduto(int IDProduto, ProdutoAtualizarDto produtoAtualizarDto)
        {
            await _produtoService.AtualizarProduto(IDProduto, produtoAtualizarDto);

            return NoContent();
        }

        [HttpDelete("{IDProduto}")]
        public async Task<IActionResult> DeletarProduto(int IDProduto) 
        {
            await _produtoService.DeletarProduto(IDProduto);

            return NoContent();
        }
    }
}
