using EDH.Core.Common;
using EDH.Core.Interfaces.IInfrastructure;
using EDH.Core.Interfaces.IInventory;
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
	private readonly UpdateStockQuantitiesDtoValidator _updateStockQuantitiesDtoValidator = new();

	public InventoryItemService(IInventoryItemRepository inventoryItemRepository, IUnitOfWork unitOfWork, ILogger<InventoryItemService> logger)
	{
		_inventoryItemRepository = inventoryItemRepository;
		_logger = logger;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<IEnumerable<GetInventoryItemsEditStockQuantitiesDto>>> GetInventoryItemsByNameAsync(string itemName)
	{
		try
		{
			string pattern = $"%{itemName}%";
			var inventoryItems = await _inventoryItemRepository
				.FindAsync(inventoryItem => EF.Functions.Like(inventoryItem.Item.Name, pattern));

			var inventoryItemsResult = inventoryItems.Select(inventoryItem => new GetInventoryItemsEditStockQuantitiesDto(inventoryItem.Id, inventoryItem.Item.Name, inventoryItem.Quantity, inventoryItem.AlertThreshold));
			
			return Result<IEnumerable<GetInventoryItemsEditStockQuantitiesDto>>.Ok(inventoryItemsResult);
		}
		catch (Exception ex)
		{
			_logger.LogCritical(ex, $"Error in {nameof(GetInventoryItemsByNameAsync)}.");
			throw;
		}
	}

	public async Task<Result<UpdateStockQuantitiesDto>> UpdateStockQuantitiesAsync(UpdateStockQuantitiesDto updateStockQuantitiesDto)
	{
		try
		{
			var validationResult = await _updateStockQuantitiesDtoValidator.ValidateAsync(updateStockQuantitiesDto);

			if (!validationResult.IsValid)
			{
				string[] errorMessages = validationResult.Errors
					.Select(e => e.ErrorMessage)
					.ToArray();
				
				return Result<UpdateStockQuantitiesDto>.Fail(errorMessages);
			}

			var inventoryItem = await _inventoryItemRepository.GetByIdAsync(updateStockQuantitiesDto.Id);

			if (inventoryItem is null)
			{
				return Result<UpdateStockQuantitiesDto>.Fail($"Inventory item {updateStockQuantitiesDto.Id} '{updateStockQuantitiesDto.ItemName}' not found");
			}

			inventoryItem.Quantity = updateStockQuantitiesDto.Quantity;
			inventoryItem.AlertThreshold = updateStockQuantitiesDto.AlertThreshold;
			inventoryItem.LastUpdated = DateTime.Now;

			_inventoryItemRepository.UpdateAsync(inventoryItem);
			await _unitOfWork.SaveChangesAsync();
			
			return Result<UpdateStockQuantitiesDto>.Ok(updateStockQuantitiesDto);
		}
		catch (Exception ex)
		{
			_logger.LogCritical(ex, $"Error in {nameof(UpdateStockQuantitiesAsync)}.");
			throw;
		}
	}
}