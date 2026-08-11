namespace Application.Dtos
{
    public class ItemCompraCriarDto
    {
        public int IDCompra { get; set; }
        public int IDProduto { get; set; }
        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
    }
}
