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
        public int? IDColecao { get; set; }
        public StatusProduto StatusProduto { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.Now;
        public DateTime DataAlteracao { get; set; }
        public Categoria Categoria { get; set; } = null!;
        public Jogo JogoTCG { get; set; } = null!;
        public Colecao? Colecao { get; set; }
        public Estoque? Estoque { get; set; }
        public ICollection<MovimentacaoEstoque> MovimentacoesEstoque { get; set; } = new List<MovimentacaoEstoque>();
        public ICollection<ItemCompra> ItensCompra { get; set; } = new List<ItemCompra>();
        [StringLength(2048)]
        [Display(Name = "Imagem do Produto")]
        public string? ImagemUrl { get; set; }
    }
}
