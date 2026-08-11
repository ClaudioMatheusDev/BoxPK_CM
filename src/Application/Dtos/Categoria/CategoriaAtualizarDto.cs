using Domain.Enums;

namespace Application.Dtos.Categoria
{
    public class CategoriaAtualizarDto
    {
        public required string Nome { get; set; }
        public string? Descricao { get; set; }
        public StatusCategoria StatusCategoria { get; set; }
    }
}
