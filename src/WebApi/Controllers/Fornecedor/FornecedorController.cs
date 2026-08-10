using Application.Dtos;
using Microsoft.AspNetCore.Mvc;
using WebApi.Service;

namespace WebApi.Controllers.Fornecedor
{
    [ApiController]
    [Route("api/fornecedor")]
    public class FornecedorController : Controller
    {

        private readonly IFornecedorService _fornecedorService;

        public FornecedorController(IFornecedorService fornecedorService)
        {
            _fornecedorService = fornecedorService;
        }

        [HttpPost]
        public async Task<IActionResult> CriarFornecedor(FornecedorCriarDto fornecedorCriarDto)
        {
            var fornecedor = await _fornecedorService.CriarFornecedor(fornecedorCriarDto);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ListarFornecedores()
        {
            var fornecedor = await _fornecedorService.ListarFornecedores();

            return Ok(fornecedor);
        }

        [HttpGet("{IDFornecedor}")]
        public async Task<IActionResult> ListarFornecedorPorID(int IDFornecedor)
        {
            var fornecedor = await _fornecedorService.ListarFornecedorPorID(IDFornecedor);

            return Ok(fornecedor);
        }

        [HttpPut("{IDFornecedor}")]
        public async Task<IActionResult> AtualizarFornecedor(int IDFornecedor, FornecedorAtualizarDto fornecedorAtualizarDto)
        {
            var fornecedor = await _fornecedorService.AtualizarFornecedor(IDFornecedor, fornecedorAtualizarDto);


            if (fornecedor == 0)
            {
                return NotFound();
            }
            return Ok();
        }

        [HttpDelete("{IDFornecedor}")]
        public async Task<IActionResult> DeletarFornecedor(int IDFornecedor)
        {
            var fornecedor = await _fornecedorService.DeletarFornecedor(IDFornecedor);
            if (!fornecedor)
            {
                return NotFound();
            }

            return Ok();

        }
    }
}
