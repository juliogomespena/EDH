using EDH.Core.Entities;
using EDH.Core.Events.Inventory;
using EDH.Core.Events.Inventory.Parameters;
using EDH.Core.Interfaces.IInfrastructure;
using EDH.Core.Interfaces.IInventory;
using EDH.Inventory.Application.Handlers.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EDH.Inventory.Application.Handlers;

public sealed class InventoryItemEventHandler : IInventoryItemEventHandler
{
	private readonly IInventoryItemRepository _inventoryItemRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IEventAggregator _eventAggregator;

	public InventoryItemEventHandler(IInventoryItemRepository inventoryItemRepository, IUnitOfWork unitOfWork, IEventAggregator eventAggregator)
	{
		_inventoryItemRepository = inventoryItemRepository;
		_unitOfWork = unitOfWork;
		_eventAggregator = eventAggregator;
	}

	public void InitializeSubscriptions()
	{
		_eventAggregator.GetEvent<CreateInventoryItemEvent>().Subscribe(HandleCreateInventoryItem);
		_eventAggregator.GetEvent<GetInventoryItemsByNameEvent>().Subscribe(HandleGetInventoryItemsByName);
	}

	public async void HandleCreateInventoryItem(CreateInventoryItemEventParameters parameters)
	{
		try
		{
			var inventoryItem = new InventoryItem
			{
				ItemId = parameters.ItemId,
				Quantity = parameters.InitialStock ?? 0,
				AlertThreshold = parameters.StockAlertThreshold,
				LastUpdated = DateTime.Now
			};

			await _inventoryItemRepository.AddAsync(inventoryItem);
			await _unitOfWork.SaveChangesAsync();

			parameters.CompletionSource.SetResult(true);
		}
		catch (Exception ex)
		{
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

			parameters.CompletionSource.SetResult(inventoryItems);
		}
		catch (Exception ex)
		{
			parameters.CompletionSource.SetException(ex);
		}
	}
}