using Domain.Enums;

namespace Application.Dtos
{
    public class FornecedorAtualizarDto
    {
        public required string Nome { get; set; }
        public required string CPFCNPJ { get; set; }
        public required string Email { get; set; }
        public required string Telefone { get; set; }
        public StatusFornecedor StatusFornecedor { get; set; }
    }
}
