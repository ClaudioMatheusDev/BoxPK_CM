using Domain.Enums;

namespace Application.Dtos
{
    public class MovimentacaoEstoqueAtualizarDto
    {
        public int IDProduto { get; set; }
        public TipoMovimentacao TipoMovimentacao { get; set; }
        public int QuantidadeAnterior { get; set; }
        public int Quantidade { get; set; }
        public int QuantidadePosterior { get; set; }
        public string? Motivo { get; set; }
        public string? Observacao { get; set; }
    }
}
