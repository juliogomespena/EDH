using EDH.Core.Exceptions;
using EDH.Core.Interfaces.IInfrastructure;
using EDH.Core.Interfaces.IInventory;
using EDH.Inventory.Application.DTOs.EditStockQuantities;
using EDH.Inventory.Application.Services.Interfaces;
using EDH.Inventory.Application.Validators.EditStockQuantities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EDH.Inventory.Application.Services;

public sealed class InventoryItemService : IInventoryItemService
{
	private readonly IInventoryItemRepository _inventoryItemRepository;
	private readonly ILogger<InventoryItemService> _logger;
	private readonly IUnitOfWork _unitOfWork;
	private readonly UpdateStockQuantitiesDtoValidator _updateStockQuantitiesDtoValidator;

	public InventoryItemService(IInventoryItemRepository inventoryItemRepository, IUnitOfWork unitOfWork, ILogger<InventoryItemService> logger)
	{
		_inventoryItemRepository = inventoryItemRepository;
		_logger = logger;
		_unitOfWork = unitOfWork;
		_updateStockQuantitiesDtoValidator = new UpdateStockQuantitiesDtoValidator();
	}

	public async Task<IEnumerable<GetInventoryItemsEditStockQuantitiesDto>> GetInventoryItemsByNameAsync(string itemName)
	{
		try
		{
			string pattern = $"%{itemName}%";
			var inventoryItems = await _inventoryItemRepository
				.FindAsync(inventoryItem => EF.Functions.Like(inventoryItem.Item.Name, pattern));

			return inventoryItems.Select(inventoryItem => new GetInventoryItemsEditStockQuantitiesDto(inventoryItem.Id, inventoryItem.Item.Name, inventoryItem.Quantity, inventoryItem.AlertThreshold));
		}
		catch (Exception ex)
		{
			_logger.LogCritical(ex, "Error while getting inventory items by name.");
			throw;
		}
	}

	public async Task UpdateStockQuantitiesAsync(UpdateStockQuantitiesDto updateStockQuantitiesDto)
	{
		try
		{
			await _unitOfWork.BeginTransactionAsync();

			var validationResult = await _updateStockQuantitiesDtoValidator.ValidateAsync(updateStockQuantitiesDto);

			if (!validationResult.IsValid)
			{
				string errorMessages = String.Join(" - ", validationResult.Errors.Select(e => e.ErrorMessage));
				throw new ValidationException(errorMessages);
			}

			var inventoryItem = await _inventoryItemRepository.GetByIdAsync(updateStockQuantitiesDto.Id);

			if (inventoryItem is null)
			{
				throw new NotFoundException(updateStockQuantitiesDto.ItemName, updateStockQuantitiesDto.Id);
			}

			inventoryItem.Quantity = updateStockQuantitiesDto.Quantity;
			inventoryItem.AlertThreshold = updateStockQuantitiesDto.AlertThreshold;
			inventoryItem.LastUpdated = DateTime.Now;

			_inventoryItemRepository.UpdateAsync(inventoryItem);
			await _unitOfWork.SaveChangesAsync();
			await _unitOfWork.CommitTransactionAsync();
		}
		catch (Exception)
		{
			await _unitOfWork.RollbackTransactionAsync();
			throw;
		}
	}
}