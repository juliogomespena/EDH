using EDH.Items.Application.DTOs.CreateItem;
using FluentValidation;

namespace EDH.Items.Application.Validators.CreateItem;

internal sealed class CreateItemInventoryDtoValidator : AbstractValidator<CreateItemInventoryDto>
{
	internal CreateItemInventoryDtoValidator()
	{
		RuleFor(itemInventory => itemInventory.InitialStock)
			.GreaterThanOrEqualTo(0).WithMessage("Item initial stock must be equal to or greater than 0");

		RuleFor(itemInventory => itemInventory.StockAlertThreshold)
			.GreaterThanOrEqualTo(0).WithMessage("Item stock alert threshold must be equal to or greater than 0");
	}
}