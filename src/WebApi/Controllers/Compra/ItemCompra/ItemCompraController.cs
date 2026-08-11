using Application.Dtos;
using Microsoft.AspNetCore.Mvc;
using WebApi.Service;

namespace WebApi.Controllers.Compra.ItemCompra
{
    [ApiController]
    [Route("api/itemcompra")]
    public class ItemCompraController : Controller
    {
        private readonly IItemCompraService _service;

        public ItemCompraController(IItemCompraService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CriarItemCompra(ItemCompraCriarDto itemCompraCriaDto)
        {
            var itemCompra = await _service.CriarItemCompra(itemCompraCriaDto);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ListarItemCompras()
        {
            var itemCompra = await _service.ListarItemCompra();

            return Ok(itemCompra);
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
            var itemCompra = await _service.AtualizarItemCompraEstoquePorID(IDItemCompra, itemCompraAtualizarDto);

            return Ok();
        }

        [HttpDelete("{IDItemCompra}")]
        public async Task<IActionResult> DeletarItemCompra(int IDItemCompra)
        {
            var itemCompra = await _service.DeletarItemCompra(IDItemCompra);

            return Ok();
        }


    }
}
