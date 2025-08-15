using EDH.Items.Application.DTOs.Request.CreateItemVariableCosts;
using FluentValidation;

namespace EDH.Items.Application.Validators.CreateItemVariableCost;

internal sealed class CreateItemVariableCostValidator : AbstractValidator<CreateItemVariableCostRequest>
{
	internal CreateItemVariableCostValidator()
	{
		RuleFor(variableCost => variableCost.Name)
			.NotEmpty().WithMessage("Item variable cost name is required")
			.MaximumLength(100).WithMessage("Item variable cost name cannot exceed 100 characters");

		RuleFor(variableCost => variableCost.Value)
			.GreaterThanOrEqualTo(0).WithMessage("Item variable cost value must be greater than or equal to zero");
	}
}