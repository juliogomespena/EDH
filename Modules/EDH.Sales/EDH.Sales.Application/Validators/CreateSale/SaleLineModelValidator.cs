using EDH.Sales.Application.DTOs.Requests.Models;
using FluentValidation;

namespace EDH.Sales.Application.Validators.CreateSale;

internal sealed class SaleLineModelValidator : AbstractValidator<SaleLineModel>
{
    internal SaleLineModelValidator()
    {
        RuleFor(line => line.ItemId)
            .NotEmpty().WithMessage("Item ID is required.")
            .GreaterThan(0).WithMessage("Item ID can't be zero.");

        RuleFor(line => line.ItemName)
            .NotEmpty().WithMessage("Item name is required")
            .MaximumLength(255).WithMessage("Item name cannot exceed 255 characters.");

        RuleFor(line => line.UnitPrice)
            .GreaterThan(0).WithMessage("Unit price must be greater than zero.");

        RuleFor(line => line.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");

        RuleFor(line => line.Subtotal)
            .GreaterThan(0).WithMessage("Subtotal must be greater than zero.");

    }
}