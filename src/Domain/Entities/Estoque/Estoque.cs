using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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
        public DateTime DataInclusao { get; set; } = DateTime.Now;
        public DateTime DataAtualizacao { get; set; }
        [JsonIgnore]
        public byte[]? RowVersion { get; set; }
        public Produto Produto { get; set; } = null!;
    }
}
