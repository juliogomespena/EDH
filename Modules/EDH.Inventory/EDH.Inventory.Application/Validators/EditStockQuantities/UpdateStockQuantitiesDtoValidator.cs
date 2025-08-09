using EDH.Inventory.Application.DTOs.EditStockQuantities;
using FluentValidation;

namespace EDH.Inventory.Application.Validators.EditStockQuantities;

internal sealed class UpdateStockQuantitiesDtoValidator : AbstractValidator<UpdateStockQuantitiesDto>
{
	internal UpdateStockQuantitiesDtoValidator()
	{
		RuleFor(inventoryItem => inventoryItem.Id)
			.NotEmpty().WithMessage("Item id is required");

		RuleFor(inventoryItem => inventoryItem.ItemName)
			.NotEmpty().WithMessage("Item name is required");

		RuleFor(inventoryItem => inventoryItem.Quantity)
			.GreaterThanOrEqualTo(0).WithMessage("Item quantity must be greater than or equal to 0");
	}
}