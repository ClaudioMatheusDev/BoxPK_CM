using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Estoque
    {
        [Key]
        public int IDEstoque { get; set; }
        public int IDProduto { get; set; }
        public int QuantidadeAtual { get; set; }
        public int QuantidadeMinima { get; set; }
        public int QuantidadeMaxima { get; set; }
        public DateTime DataInclusao { get; set; } = DateTime.Now.AddHours(-3);
        public DateTime DataAtualizacao { get; set; }
    }
}
