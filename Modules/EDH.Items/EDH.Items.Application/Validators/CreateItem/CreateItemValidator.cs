using EDH.Items.Application.DTOs.Request.CreateItem;
using EDH.Items.Application.Validators.CreateItemVariableCost;
using FluentValidation;

namespace EDH.Items.Application.Validators.CreateItem;

internal sealed class CreateItemValidator : AbstractValidator<CreateItemRequest>
{
	internal CreateItemValidator()
	{
		RuleFor(item => item.Name)
			.NotEmpty().WithMessage("Item name is required")
			.MaximumLength(100).WithMessage("Item name must not exceed 100 characters");

		RuleFor(item => item.Description)
			.MaximumLength(500).WithMessage("Item description cannot exceed 500 characters");

		RuleFor(item => item.SellingPrice)
			.GreaterThanOrEqualTo(0).WithMessage("Item selling price must be greater than zero");

		RuleForEach(item => item.VariableCosts).SetValidator(new CreateItemVariableCostValidator());
	}
}