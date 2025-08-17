using EDH.Core.Common;
using EDH.Core.Entities;
using EDH.Core.Events.Inventory;
using EDH.Core.Events.Inventory.Parameters;
using EDH.Core.Interfaces.IInfrastructure;
using EDH.Core.Interfaces.IInventory;
using EDH.Inventory.Application.Handlers.Interfaces;
using Microsoft.EntityFrameworkCore;
using  EDH.Core.Events.Abstractions;
using EDH.Core.ValueObjects;
using Microsoft.Extensions.Logging;

namespace EDH.Inventory.Application.Handlers;

public sealed class InventoryItemEventHandler : IInventoryItemEventHandler
{
	private readonly IInventoryItemRepository _inventoryItemRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IEventAggregator _eventAggregator;
	private readonly ILogger<InventoryItemEventHandler> _logger;

	public InventoryItemEventHandler(IInventoryItemRepository inventoryItemRepository, IUnitOfWork unitOfWork, IEventAggregator eventAggregator, ILogger<InventoryItemEventHandler> logger)
	{
		_inventoryItemRepository = inventoryItemRepository;
		_unitOfWork = unitOfWork;
		_eventAggregator = eventAggregator;
		_logger = logger;
	}

	public void InitializeSubscriptions()
	{
		_eventAggregator.Subscribe<CreateInventoryItemEvent, CreateInventoryItemEventParameters>(
				HandleCreateInventoryItem);
        
		_eventAggregator.Subscribe<GetInventoryItemsByNameEvent, GetInventoryItemsByNameEventParameters>(
				HandleGetInventoryItemsByName);
		
		_eventAggregator.Subscribe<GetInventoryItemsByItemIdEvent, GetInventoryItemByItemIdParameters>(
			HandleGetInventoryItemByItemId);

		_eventAggregator.Subscribe<DecreaseInventoryItemByItemIdEvent, DecreaseInventoryItemByItemIdParameters>(
			HandleDecreaseInventoryItemByItemId);
	}

	public async void HandleCreateInventoryItem(CreateInventoryItemEventParameters parameters)
	{
		try
		{
			var inventoryItem = new InventoryItem
			{
				ItemId = parameters.ItemId,
				Quantity = Quantity.FromValue(parameters.InitialStock ?? 0),
				AlertThreshold = parameters.StockAlertThreshold.HasValue 
					? Quantity.FromValue(parameters.StockAlertThreshold.Value) 
					: null,
				LastUpdated = DateTime.Now
			};

			await _inventoryItemRepository.AddAsync(inventoryItem);
			await _unitOfWork.SaveChangesAsync();

			parameters.CompletionSource.SetResult(Result<InventoryItem>.Ok(inventoryItem));
		}
		catch (Exception ex)
		{
			_logger.LogCritical(ex, $"Error in {nameof(HandleCreateInventoryItem)}.");
			parameters.CompletionSource.SetException(ex);
		}
	}
	
	public async void HandleGetInventoryItemsByName(GetInventoryItemsByNameEventParameters parameters)
	{
		try
		{
			string pattern = $"%{parameters.ItemName}%";
			var inventoryItems = await _inventoryItemRepository
				.FindAsync(inventoryItem => EF.Functions.Like(inventoryItem.Item.Name, pattern));

			parameters.CompletionSource.SetResult(Result<IEnumerable<InventoryItem>>.Ok(inventoryItems));
		}
		catch (Exception ex)
		{
			_logger.LogCritical(ex, $"Error in {nameof(HandleGetInventoryItemsByName)}.");
			parameters.CompletionSource.SetException(ex);
		}
	}
	
	public async void HandleGetInventoryItemByItemId(GetInventoryItemByItemIdParameters parameters)
	{
		try
		{
			var inventoryItem = (await _inventoryItemRepository
				.FindAsync(inventoryItem => inventoryItem.ItemId == parameters.ItemId))
				.FirstOrDefault();

			parameters.CompletionSource.SetResult(Result<InventoryItem?>.Ok(inventoryItem));
		}
		catch (Exception ex)
		{
			_logger.LogCritical(ex, $"Error in {nameof(HandleGetInventoryItemByItemId)}.");
			parameters.CompletionSource.SetException(ex);
		}
	}
	
	public async void HandleDecreaseInventoryItemByItemId(DecreaseInventoryItemByItemIdParameters parameters)
	{
		try
		{
			var inventoryItem = (await _inventoryItemRepository
					.FindAsync(inventoryItem => inventoryItem.ItemId == parameters.ItemId))
				.FirstOrDefault();

			if (inventoryItem is null)
			{
				parameters.CompletionSource.SetResult(Result.Fail($"Inventory for item '{parameters.ItemId}' not found"));
				return;
			}

			inventoryItem.Quantity = inventoryItem.Quantity.Subtract(Quantity.FromValue(parameters.Amount));
			await _unitOfWork.SaveChangesAsync();

			parameters.CompletionSource.SetResult(Result.Ok());
		}
		catch (Exception ex)
		{
			_logger.LogCritical(ex, $"Error in {nameof(HandleDecreaseInventoryItemByItemId)}.");
			parameters.CompletionSource.SetException(ex);
		}
	}
}