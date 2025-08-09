using EDH.Sales.Application.DTOs.RecordSale;
using FluentValidation;

namespace EDH.Sales.Application.Validators.CreateSale;

internal sealed class CreateSaleDtoValidator : AbstractValidator<SaleRecordSaleDto>
{
    internal CreateSaleDtoValidator()
    {
        RuleFor(sale => sale.TotalValue)
            .GreaterThan(0).WithMessage("Total value must be greater than zero");
        
        RuleFor(sale => sale.SaleLines)
            .NotEmpty().WithMessage("Sale lines must be provided");
        
        RuleForEach(sale => sale.SaleLines)
            .SetValidator(new SaleLineRecordSaleDtoValidator());
    }
}