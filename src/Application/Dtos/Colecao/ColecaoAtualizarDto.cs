using Domain.Enums;

namespace Application.Dtos
{
    public class ColecaoAtualizarDto
    {
        public required string Nome { get; set; }
        public string? Sigla { get; set; }
        public int IDJogoTCG { get; set; }
        public DateTime DataLancamento { get; set; }
        public StatusColecao StatusColecao { get; set; }
    }
}
