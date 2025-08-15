using EDH.Sales.Application.Validators.ItemQuantity.Models;
using FluentValidation;

namespace EDH.Sales.Application.Validators.ItemQuantity;

public sealed class ItemQuantityValidator : AbstractValidator<ItemQuantityValidatorInputModel>
{
    public ItemQuantityValidator()
    {
        RuleFor(i => i.ItemQuantity)
            .NotEmpty()
            .When(i => i.HasSelectedItem)
            .WithMessage("Item quantity is required.");
        
        RuleFor(i => i.ItemQuantity)
            .Must(BeValidInteger)
            .When(i => i.HasSelectedItem && 
                       !String.IsNullOrWhiteSpace(i.ItemQuantity))
            .WithMessage("Item quantity must be a valid integer greater than zero.");
        
        RuleFor(i => i)
            .Must(HaveSufficientStock)
            .When(i => i.HasSelectedItem && 
                       !String.IsNullOrWhiteSpace(i.ItemQuantity) &&
                       BeValidInteger(i.ItemQuantity))
            .WithMessage(i => $"Available in stock: {i.AvailableStock}.");
    }

    private static bool HaveSufficientStock(ItemQuantityValidatorInputModel model)
    {
        if (!Int32.TryParse(model.ItemQuantity, out int quantity)) return false;

        return quantity <= model.AvailableStock;
    }

    private static bool BeValidInteger(string? value) => 
        Int32.TryParse(value, out int parsedValue) && parsedValue > 0;
 
}