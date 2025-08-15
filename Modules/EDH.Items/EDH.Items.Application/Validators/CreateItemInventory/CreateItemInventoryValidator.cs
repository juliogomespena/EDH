using EDH.Items.Application.DTOs.Request.CreateItemInventory;
using FluentValidation;

namespace EDH.Items.Application.Validators.CreateItemInventory;

internal sealed class CreateItemInventoryValidator : AbstractValidator<CreateItemInventoryRequest>
{
	internal CreateItemInventoryValidator()
	{
		RuleFor(itemInventory => itemInventory.InitialStock)
			.GreaterThanOrEqualTo(0).WithMessage("Item initial stock must be equal to or greater than 0");

		RuleFor(itemInventory => itemInventory.StockAlertThreshold)
			.GreaterThanOrEqualTo(0).WithMessage("Item stock alert threshold must be equal to or greater than 0");
	}
}