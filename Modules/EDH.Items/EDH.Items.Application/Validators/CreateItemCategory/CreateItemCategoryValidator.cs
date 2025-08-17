using EDH.Items.Application.DTOs.Requests.CreateItemCategory;
using FluentValidation;

namespace EDH.Items.Application.Validators.CreateItemCategory;

internal sealed class CreateItemCategoryValidator : AbstractValidator<CreateItemCategoryRequest>
{
	internal CreateItemCategoryValidator()
	{
		RuleFor(itemCategory => itemCategory.Name)
			.NotEmpty().WithMessage("Item category name is required")
			.MaximumLength(100).WithMessage("Item category name must not exceed 100 characters");

		RuleFor(itemCategory => itemCategory.Description)
			.MaximumLength(500).WithMessage("Item category description cannot exceed 500 characters");
	}
}