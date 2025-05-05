using EDH.Core.Entities;
using EDH.Core.Events.Inventory;
using EDH.Core.Events.Inventory.Parameters;
using EDH.Sales.Application.DTOs;
using EDH.Sales.Application.Services.Interfaces;

namespace EDH.Sales.Application.Services;

public sealed class SalesService : ISalesService
{
    private readonly IEventAggregator _eventAggregator;

    public SalesService(IEventAggregator eventAggregator)
    {
        _eventAggregator = eventAggregator;
    }
    public async Task<IEnumerable<GetInventoryItemsRecordSaleDto>> GetInventoryItemsByNameAsync(string itemName)
    {
        try
        {
            var completionSource = new TaskCompletionSource<IEnumerable<InventoryItem>>();
            
            _eventAggregator.GetEvent<GetInventoryItemsByNameEvent>().Publish(new GetInventoryItemsByNameEventParameters(itemName)
            {
                CompletionSource = completionSource
            });

            var inventoryItems = await completionSource.Task;

            return inventoryItems.Select(item => new GetInventoryItemsRecordSaleDto(item.Id, item.Item.Name, new GetItemRecordSaleDto(item.Item.SellingPrice, item.Item.ItemVariableCosts.Sum(vc => vc.Value))));
        }
        catch (Exception)
        {
            throw;
        }
    }
}