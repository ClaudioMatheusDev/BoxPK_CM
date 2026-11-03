using Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Service;

namespace WebApi.Controllers.Compra
{
    [ApiController]
    [Route("api/compra")]
    [Authorize]
    public class CompraController : Controller
    {
        private readonly ICompraService _compraService;

        public CompraController(ICompraService compraService)
        {
            _compraService = compraService;
        }

        [HttpPost]
        public async Task<IActionResult> CriarCompra(CompraCriarDto compraCriaDto)
        {
            var compra = await _compraService.CriarCompra(compraCriaDto);

            return StatusCode(StatusCodes.Status201Created, new { id = compra });
        }

        [HttpGet]
        public async Task<IActionResult> ListarCompras([FromQuery] int? page, [FromQuery] int? pageSize)
        {
            var resultado = await _compraService.ListarCompras(page, pageSize);

            return Ok(resultado);
        }

        [HttpGet("{IDCompra}")]
        public async Task<IActionResult> ListarCompraPorID(int IDCompra)
        {
            var compra = await _compraService.ListarCompraPorID(IDCompra);

            return Ok(compra);
        }

        [HttpPut("{IDCompra}")]
        public async Task<IActionResult> AtualizarCompra(int IDCompra, CompraAtualizarDto compraAtualizarDto)
        {
            await _compraService.AtualizarCompraPorID(IDCompra, compraAtualizarDto);

            return NoContent();
        }

        [HttpDelete("{IDCompra}")]
        public async Task<IActionResult> DeletarCompra(int IDCompra)
        {
            await _compraService.DeletarCompra(IDCompra);

            return NoContent();
        }


    }
}
