using Domain.Enums;

namespace Application.Dtos
{
    public class CompraCriarDto
    {
        public int IDFornecedor { get; set; }
        public DateTime DataCompra { get; set; }
        public decimal ValorTotal { get; set; }
        public string? Observacao { get; set; }
    }
}
