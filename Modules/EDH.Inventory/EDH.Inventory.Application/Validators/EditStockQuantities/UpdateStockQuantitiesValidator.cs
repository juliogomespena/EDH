using EDH.Inventory.Application.DTOs.Requests.UpdateStockQuantities;
using FluentValidation;

namespace EDH.Inventory.Application.Validators.EditStockQuantities;

internal sealed class UpdateStockQuantitiesValidator : AbstractValidator<UpdateStockQuantitiesRequest>
{
	internal UpdateStockQuantitiesValidator()
	{
		RuleFor(inventoryItem => inventoryItem.Id)
			.NotEmpty().WithMessage("Item id is required");

		RuleFor(inventoryItem => inventoryItem.ItemName)
			.NotEmpty().WithMessage("Item name is required");

		RuleFor(inventoryItem => inventoryItem.Quantity)
			.GreaterThanOrEqualTo(0).WithMessage("Item quantity must be greater than or equal to 0");
	}
}