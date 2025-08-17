using EDH.Core.Common;
using EDH.Core.Interfaces.IInfrastructure;
using EDH.Core.Interfaces.IInventory;
using EDH.Core.ValueObjects;
using EDH.Inventory.Application.DTOs.Requests.StockAdjustmentCalculation;
using EDH.Inventory.Application.DTOs.Requests.UpdateStockQuantities;
using EDH.Inventory.Application.DTOs.Responses.GetInventoryItems;
using EDH.Inventory.Application.DTOs.Responses.StockAdjustmentCalculation;
using EDH.Inventory.Application.DTOs.Responses.UpdateStockQuantities;
using EDH.Inventory.Application.Services.Interfaces;
using EDH.Inventory.Application.Validators.EditStockQuantities;
using EDH.Inventory.Core.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EDH.Inventory.Application.Services;

public sealed class InventoryItemService : IInventoryItemService
{
	private readonly IInventoryItemRepository _inventoryItemRepository;
	private readonly ILogger<InventoryItemService> _logger;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IStockAdjustmentCalculationService _stockAdjustmentCalculationService;
	private readonly UpdateStockQuantitiesValidator _updateStockQuantitiesValidator = new();

	public InventoryItemService(IInventoryItemRepository inventoryItemRepository, IUnitOfWork unitOfWork, IStockAdjustmentCalculationService stockAdjustmentCalculationService, ILogger<InventoryItemService> logger)
	{
		_inventoryItemRepository = inventoryItemRepository;
		_logger = logger;
		_unitOfWork = unitOfWork;
		_stockAdjustmentCalculationService = stockAdjustmentCalculationService;
	}

	public async Task<Result<IEnumerable<GetInventoryItemsResponse>>> GetInventoryItemsByNameAsync(string itemName)
	{
		try
		{
			string pattern = $"%{itemName}%";
			var inventoryItems = await _inventoryItemRepository
				.FindAsync(inventoryItem => EF.Functions.Like(inventoryItem.Item.Name, pattern));

			var inventoryItemsResult = inventoryItems.Select(inventoryItem => new GetInventoryItemsResponse(inventoryItem.Id, inventoryItem.Item.Name, inventoryItem.Quantity, inventoryItem.AlertThreshold));
			
			return Result<IEnumerable<GetInventoryItemsResponse>>.Ok(inventoryItemsResult);
		}
		catch (Exception ex)
		{
			_logger.LogCritical(ex, $"Error in {nameof(GetInventoryItemsByNameAsync)}.");
			throw;
		}
	}

	public Result<StockAdjustmentCalculationResponse> CalculateStockAdjustment(StockAdjustmentCalculationRequest request)
	{
		try
		{
			var result = _stockAdjustmentCalculationService
				.Calculate(Quantity.FromValue(request.CurrentQuantity), request.Adjustment);
			
			if (result.IsFailure || result.Value is null)
				return Result<StockAdjustmentCalculationResponse>.Fail(result.Errors[0]);
			
			return Result<StockAdjustmentCalculationResponse>.Ok(new StockAdjustmentCalculationResponse(result.Value.Quantity.Value));
		}
		catch (ArgumentException ex)
		{
			_logger.LogCritical(ex, $"Error in {nameof(CalculateStockAdjustment)}.");
			return Result<StockAdjustmentCalculationResponse>.Fail(ex);
		}
	}

	public async Task<Result<UpdateStockQuantitiesResponse>> UpdateStockQuantitiesAsync(UpdateStockQuantitiesRequest request)
	{
		try
		{
			var validationResult = await _updateStockQuantitiesValidator.ValidateAsync(request);

			if (!validationResult.IsValid)
			{
				string[] errorMessages = validationResult.Errors
					.Select(e => e.ErrorMessage)
					.ToArray();
				
				return Result<UpdateStockQuantitiesResponse>.Fail(errorMessages);
			}

			var inventoryItem = await _inventoryItemRepository.GetByIdAsync(request.Id);

			if (inventoryItem is null)
			{
				return Result<UpdateStockQuantitiesResponse>.Fail($"Inventory item {request.Id} '{request.ItemName}' not found");
			}

			inventoryItem.Quantity = Quantity.FromValue(request.Quantity);
			inventoryItem.AlertThreshold = request.AlertThreshold.HasValue 
				? Quantity.FromValue(request.AlertThreshold.Value) 
				: null;
			inventoryItem.LastUpdated = DateTime.Now;

			_inventoryItemRepository.UpdateAsync(inventoryItem);
			await _unitOfWork.SaveChangesAsync();
			
			return Result<UpdateStockQuantitiesResponse>.Ok(new UpdateStockQuantitiesResponse(request.Id, request.ItemName));
		}
		catch (Exception ex)
		{
			_logger.LogCritical(ex, $"Error in {nameof(UpdateStockQuantitiesAsync)}.");
			throw;
		}
	}
}