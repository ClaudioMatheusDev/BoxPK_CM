using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Produto
    {
        [Key]
        public int IDProduto { get; set; }
        public required String Nome { get; set; }
        public String? Descricao { get; set; }
        public decimal PrecoCusto { get; set; }
        public decimal PrecoVenda { get; set; }
        public int IDCategoria { get; set; }
        public int IDJogoTCG { get; set; }
        public StatusProduto StatusProduto { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.Now.AddHours(-3);
        public DateTime DataAlteracao { get; set; }
          }
}
