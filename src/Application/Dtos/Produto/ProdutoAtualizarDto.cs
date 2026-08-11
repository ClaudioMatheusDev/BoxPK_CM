using Domain.Enums;

namespace Application.Dtos.Produto
{
    public class ProdutoAtualizarDto
    {
        public required string Nome { get; set; }
        public string? Descricao { get; set; }
        public decimal PrecoCusto { get; set; }
        public decimal PrecoVenda { get; set; }
        public int IDCategoria { get; set; }
        public int IDJogoTCG { get; set; }
        public int? IDColecao { get; set; }
        public StatusProduto StatusProduto { get; set; }
        public string? ImagemUrl { get; set; }
    }
}
