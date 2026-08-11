namespace Application.Dtos
{
    public class EstoqueAtualizarDto
    {
        public int IDProduto { get; set; }
        public int QuantidadeAtual { get; set; }
        public int QuantidadeMinima { get; set; }
        public int QuantidadeMaxima { get; set; }
    }
}
