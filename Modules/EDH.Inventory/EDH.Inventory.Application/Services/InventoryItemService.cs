using EDH.Core.Common;
using EDH.Core.Interfaces.IInfrastructure;
using EDH.Core.Interfaces.IInventory;
using EDH.Core.ValueObjects;
using EDH.Inventory.Application.DTOs.EditStockQuantities;
using EDH.Inventory.Application.Services.Interfaces;
using EDH.Inventory.Application.Validators.EditStockQuantities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EDH.Inventory.Application.Services;

public sealed class InventoryItemService : IInventoryItemService
{
	private readonly IInventoryItemRepository _inventoryItemRepository;
	private readonly ILogger<InventoryItemService> _logger;
	private readonly IUnitOfWork _unitOfWork;
	private readonly UpdateStockQuantitiesValidator _updateStockQuantitiesValidator = new();

	public InventoryItemService(IInventoryItemRepository inventoryItemRepository, IUnitOfWork unitOfWork, ILogger<InventoryItemService> logger)
	{
		_inventoryItemRepository = inventoryItemRepository;
		_logger = logger;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<IEnumerable<GetInventoryItems>>> GetInventoryItemsByNameAsync(string itemName)
	{
		try
		{
			string pattern = $"%{itemName}%";
			var inventoryItems = await _inventoryItemRepository
				.FindAsync(inventoryItem => EF.Functions.Like(inventoryItem.Item.Name, pattern));

			var inventoryItemsResult = inventoryItems.Select(inventoryItem => new GetInventoryItems(inventoryItem.Id, inventoryItem.Item.Name, inventoryItem.Quantity, inventoryItem.AlertThreshold));
			
			return Result<IEnumerable<GetInventoryItems>>.Ok(inventoryItemsResult);
		}
		catch (Exception ex)
		{
			_logger.LogCritical(ex, $"Error in {nameof(GetInventoryItemsByNameAsync)}.");
			throw;
		}
	}

	public async Task<Result<UpdateStockQuantities>> UpdateStockQuantitiesAsync(UpdateStockQuantities updateStockQuantities)
	{
		try
		{
			var validationResult = await _updateStockQuantitiesValidator.ValidateAsync(updateStockQuantities);

			if (!validationResult.IsValid)
			{
				string[] errorMessages = validationResult.Errors
					.Select(e => e.ErrorMessage)
					.ToArray();
				
				return Result<UpdateStockQuantities>.Fail(errorMessages);
			}

			var inventoryItem = await _inventoryItemRepository.GetByIdAsync(updateStockQuantities.Id);

			if (inventoryItem is null)
			{
				return Result<UpdateStockQuantities>.Fail($"Inventory item {updateStockQuantities.Id} '{updateStockQuantities.ItemName}' not found");
			}

			inventoryItem.Quantity = Quantity.FromValue(updateStockQuantities.Quantity);
			inventoryItem.AlertThreshold = updateStockQuantities.AlertThreshold.HasValue 
				? Quantity.FromValue(updateStockQuantities.AlertThreshold.Value) 
				: null;
			inventoryItem.LastUpdated = DateTime.Now;

			_inventoryItemRepository.UpdateAsync(inventoryItem);
			await _unitOfWork.SaveChangesAsync();
			
			return Result<UpdateStockQuantities>.Ok(updateStockQuantities);
		}
		catch (Exception ex)
		{
			_logger.LogCritical(ex, $"Error in {nameof(UpdateStockQuantitiesAsync)}.");
			throw;
		}
	}
}