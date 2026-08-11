using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class MovimentacaoEstoque
    {
        [Key]
        public int IDMovimentacao { get; set; }
        public int IDProduto { get; set; }
        public TipoMovimentacao TipoMovimentacao { get; set; }
        public int QuantidadeAnterior { get; set; }
        public int Quantidade { get; set; }
        public int QuantidadePosterior { get; set; }
        public string? Motivo { get; set; }
        public string? Observacao { get; set; }
        public DateTime DataMovimentacao { get; set; } = DateTime.Now;
    }
}
