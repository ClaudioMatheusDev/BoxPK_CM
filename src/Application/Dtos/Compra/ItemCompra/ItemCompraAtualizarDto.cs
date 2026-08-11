using FluentValidation;

namespace Application.Dtos
{
    public class ItemCompraAtualizarDto
    {
        public int IDCompra { get; set; }
        public int IDProduto { get; set; }
        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
    }

    public class ItemCompraAtualizarDtoValidator : AbstractValidator<ItemCompraAtualizarDto>
    {
        public ItemCompraAtualizarDtoValidator()
        {
            RuleFor(q => q.IDCompra).NotEmpty().WithMessage("ID de compra é obrigatório.");
            RuleFor(q => q.IDProduto).NotEmpty().WithMessage("ID de produto é obrigatório.");
            RuleFor(q => q.Quantidade).NotEmpty().GreaterThan(0).WithMessage("Quantidade não pode ser menor ou igual a zero.");
            RuleFor(q => q.ValorUnitario).NotEmpty().GreaterThan(0).WithMessage("O valor unitario não pode ser igual a zero.");
        }
    }
}
