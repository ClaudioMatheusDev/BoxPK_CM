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
        public async Task<IActionResult> CriarCompra(CompraCriaDto compraCriaDto)
        {
            var compra = await _compraService.CriarCompra(compraCriaDto);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ListarCompras()
        {
            var compra = await _compraService.ListarCompras();

            return Ok(compra);
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
            var compra = await _compraService.AtualizarCompraPorID(IDCompra, compraAtualizarDto);

            return Ok();
        }

        [HttpDelete("{IDCompra}")]
        public async Task<IActionResult> DeletarCompra(int IDCompra)
        {
            var compra = await _compraService.DeletarCompra(IDCompra);

            return Ok();
        }


    }
}
