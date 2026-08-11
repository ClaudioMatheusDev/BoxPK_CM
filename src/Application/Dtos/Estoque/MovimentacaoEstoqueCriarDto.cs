using Domain.Enums;

namespace Application.Dtos
{
    public class MovimentacaoEstoqueCriarDto
    {
        public int IDProduto { get; set; }
        public TipoMovimentacao TipoMovimentacao { get; set; }
        public int Quantidade { get; set; }
        public string? Motivo { get; set; }
        public string? Observacao { get; set; }
    }
}
