using EDH.Items.Application.DTOs.CreateItem;
using FluentValidation;

namespace EDH.Items.Application.Validators.CreateItem;

internal sealed class CreateItemDtoValidator : AbstractValidator<CreateItemDto>
{
	internal CreateItemDtoValidator()
	{
		RuleFor(item => item.Name)
			.NotEmpty().WithMessage("Item name is required")
			.MaximumLength(100).WithMessage("Item name must not exceed 100 characters");

		RuleFor(item => item.Description)
			.MaximumLength(500).WithMessage("Item description cannot exceed 500 characters");

		RuleFor(item => item.SellingPrice)
			.GreaterThanOrEqualTo(0).WithMessage("Item selling price must be greater than zero");

		RuleForEach(item => item.VariableCosts).SetValidator(new CreateItemVariableCostDtoValidator());
	}
}