using Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Service;

namespace WebApi.Controllers.Compra.ItemCompra
{
    [ApiController]
    [Route("api/itemcompra")]
    [Authorize]
    public class ItemCompraController : Controller
    {
        private readonly IItemCompraService _service;

        public ItemCompraController(IItemCompraService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CriarItemCompra(ItemCompraCriarDto itemCompraCriarDto)
        {
            var itemCompra = await _service.CriarItemCompra(itemCompraCriarDto);

            return StatusCode(StatusCodes.Status201Created, new { id = itemCompra });
        }

        [HttpGet]
        public async Task<IActionResult> ListarItemCompras([FromQuery] int? page, [FromQuery] int? pageSize)
        {
            var resultado = await _service.ListarItemCompra(page, pageSize);

            return Ok(resultado);
        }

        [HttpGet("{IDItemCompra}")]
        public async Task<IActionResult> ListarItemCompraPorID(int IDItemCompra)
        {
            var itemCompra = await _service.ListarItemCompraPorID(IDItemCompra);

            return Ok(itemCompra);
        }

        [HttpPut("{IDItemCompra}")]
        public async Task<IActionResult> AtualizarItemCompra(int IDItemCompra, ItemCompraAtualizarDto itemCompraAtualizarDto)
        {
            await _service.AtualizarItemCompraPorID(IDItemCompra, itemCompraAtualizarDto);

            return NoContent();
        }

        [HttpDelete("{IDItemCompra}")]
        public async Task<IActionResult> DeletarItemCompra(int IDItemCompra)
        {
            await _service.DeletarItemCompra(IDItemCompra);

            return NoContent();
        }


    }
}
