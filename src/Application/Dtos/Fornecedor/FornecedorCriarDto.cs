namespace Application.Dtos
{
    public class FornecedorCriarDto
    {
        public required string Nome { get; set; }
        public required string CPFCNPJ { get; set; }
        public required string Email { get; set; }
        public required string Telefone { get; set; }
    }
}
