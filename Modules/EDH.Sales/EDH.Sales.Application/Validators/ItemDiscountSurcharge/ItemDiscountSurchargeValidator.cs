using EDH.Core.Extensions;
using FluentValidation;

namespace EDH.Sales.Application.Validators.ItemDiscountSurcharge;

public sealed class ItemDiscountSurchargeValidator : AbstractValidator<string>
{
    public ItemDiscountSurchargeValidator()
    {
        RuleFor(i =>i)
            .Must(BeValidDecimal)
            .When(i => !String.IsNullOrWhiteSpace(i))
            .WithMessage("Only numeric values allowed.");
    }

    private bool BeValidDecimal(string? value) => value?.TryToDecimal(out _) ?? false;
}