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

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ListarProdutos()
        {
            var produto = await _produtoService.ListarProdutos();

            return Ok(produto);
        }

        [HttpGet("{IDproduto}")]
        public async Task<IActionResult> ListarProdutosPorID(int IDproduto)
        {
            var produto = await _produtoService.ListarProdutoPorID(IDproduto);

            return Ok(produto);
        }

        [HttpPut("{IDProduto}")]

        public async Task<IActionResult> AtualiarProduto(int IDProduto, ProdutoAtualizarDto produtoAtualizarDto)
        {
            var produto = await _produtoService.AtualizarProduto(IDProduto, produtoAtualizarDto);

            return Ok();
        }

        [HttpDelete("{IDProduto}")]
        public async Task<IActionResult> DeletarProduto(int IDProduto) 
        {
            var produto = await _produtoService.DeletarProduto(IDProduto);

            return Ok();
        }
    }
}
