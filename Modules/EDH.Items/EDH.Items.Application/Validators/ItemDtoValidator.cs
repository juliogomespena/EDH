using EDH.Items.Application.DTOs;
using FluentValidation;

namespace EDH.Items.Application.Validators;

public sealed class ItemDtoValidator : AbstractValidator<ItemDto>
{
	public ItemDtoValidator()
	{
		RuleFor(item => item.Name)
			.NotEmpty().WithMessage("Item name is required")
			.MaximumLength(100).WithMessage("Item name must not exceed 100 characters");

		RuleFor(item => item.Description)
			.MaximumLength(500).WithMessage("Item description cannot exceed 500 characters");

		RuleFor(item => item.SellingPrice)
			.GreaterThanOrEqualTo(0).WithMessage("Item selling price must be greater than zero");

		RuleForEach(item => item.VariableCosts).SetValidator(new ItemVariableCostDtoValidator());
	}
}

public sealed class ItemVariableCostDtoValidator : AbstractValidator<ItemVariableCostDto>
{
	public ItemVariableCostDtoValidator()
	{
		RuleFor(x => x.Name)
			.NotEmpty().WithMessage("Item variable cost name is required")
			.MaximumLength(100).WithMessage("Item variable cost name cannot exceed 100 characters");

		RuleFor(x => x.Value)
			.GreaterThanOrEqualTo(0).WithMessage("Item variable cost value must be greater than or equal to zero");
	}
}