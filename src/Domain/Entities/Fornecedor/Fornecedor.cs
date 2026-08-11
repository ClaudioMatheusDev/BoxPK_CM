using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Fornecedor
    {
        [Key]
        public int IDFornecedor { get; set; }
        public required string Nome { get; set; }
        public required string CPFCNPJ { get; set; }
        public required string Email { get; set; }
        public required string Telefone { get; set; }
        public StatusFornecedor StatusFornecedor { get; set; }
        public DateTime DataCadastro { get; set; } = DateTime.Now;
        public DateTime DataAtualizacao { get; set; }
        public ICollection<Compra> Compras { get; set; } = new List<Compra>();
    }
}
