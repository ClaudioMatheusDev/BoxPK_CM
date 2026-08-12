using Application.Dtos;
using Application.Dtos.Categoria;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Service;

namespace WebApi.Controllers.Categoria
{
    [ApiController]
    [Route("api/categoria")]
    [Authorize]
    public class CategoriaController : Controller
    {

        private readonly ICategoriaService _categoriaService;

        public CategoriaController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        [HttpPost]
        public async Task<IActionResult> CriarCategoria(CategoriaCriarDto categoriaCriarDto)
        {
            var categoria = await _categoriaService.CriarCategoria(categoriaCriarDto);


            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> BuscarCategorias()
        {
            var categoria = await _categoriaService.ListarCategorias();

            if (categoria is null)
            {
                return NotFound();
            }

            return Ok(categoria);
        }

        [HttpGet("{IDCategoria}")]
        public async Task<IActionResult> BuscarCategoriasPorID(int IDCategoria)
        {
            var categoria = await _categoriaService.ListarCategoriaPorID(IDCategoria);

            if (categoria is null)
            {
                return NotFound();
            }

            return Ok(categoria);
        }

        [HttpPut("{IDCategoria}")]
        public async Task<IActionResult> AtualizarCategoria(int IDCategoria, CategoriaAtualizarDto categoriaAtualizarDto)
        {
            var categoria = await _categoriaService.AtualizarCategoria(IDCategoria, categoriaAtualizarDto);

            if (categoria == 0)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpDelete("{IDCategoria}")]
        public async Task<IActionResult> DeletarCategoria(int IDCategoria)
        {
            var categoria = await _categoriaService.DeletarCategoria(IDCategoria);

            if (!categoria)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
