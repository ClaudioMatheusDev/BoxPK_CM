using Domain.Enums;

namespace Application.Dtos.Jogo
{
    public class JogoAtualizarDto
    {
        public required string Nome { get; set; }
        public string? Fabricante { get; set; }
        public StatusJogo StatusJogo { get; set; }
    }
}
