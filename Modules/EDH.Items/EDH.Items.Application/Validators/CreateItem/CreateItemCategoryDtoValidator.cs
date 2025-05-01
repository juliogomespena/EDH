using EDH.Items.Application.DTOs.CreateItem;
using FluentValidation;

namespace EDH.Items.Application.Validators.CreateItem;

internal sealed class CreateItemCategoryDtoValidator : AbstractValidator<CreateItemCategoryDto>
{
	internal CreateItemCategoryDtoValidator()
	{
		RuleFor(itemCategory => itemCategory.Name)
			.NotEmpty().WithMessage("Item category name is required")
			.MaximumLength(100).WithMessage("Item category name must not exceed 100 characters");

		RuleFor(itemCategory => itemCategory.Description)
			.MaximumLength(500).WithMessage("Item category description cannot exceed 500 characters");
	}
}