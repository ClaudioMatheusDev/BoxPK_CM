using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Compra
    {
        [Key]
        public int IDCompra { get; set; }
        public int IDFornecedor { get; set; }
        public DateTime DataCompra { get; set; }
        public decimal ValorTotal { get; set; }
        public string? Observacao { get; set; }
        public StatusCompra StatusCompra { get; set; }
    }
}
