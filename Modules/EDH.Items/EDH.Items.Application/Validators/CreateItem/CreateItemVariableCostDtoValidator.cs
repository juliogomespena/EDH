using EDH.Items.Application.DTOs.CreateItem;
using FluentValidation;

namespace EDH.Items.Application.Validators.CreateItem;

internal sealed class CreateItemVariableCostDtoValidator : AbstractValidator<CreateItemVariableCostDto>
{
	internal CreateItemVariableCostDtoValidator()
	{
		RuleFor(variableCost => variableCost.Name)
			.NotEmpty().WithMessage("Item variable cost name is required")
			.MaximumLength(100).WithMessage("Item variable cost name cannot exceed 100 characters");

		RuleFor(variableCost => variableCost.Value)
			.GreaterThanOrEqualTo(0).WithMessage("Item variable cost value must be greater than or equal to zero");
	}
}