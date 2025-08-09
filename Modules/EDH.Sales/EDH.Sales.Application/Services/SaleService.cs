using EDH.Core.Entities;
using EDH.Core.Events.Inventory;
using EDH.Core.Events.Inventory.Parameters;
using EDH.Core.Interfaces.IInfrastructure;
using EDH.Core.Interfaces.ISales;
using EDH.Sales.Application.DTOs.RecordSale;
using EDH.Sales.Application.Services.Interfaces;
using EDH.Sales.Application.Validators.CreateSale;
using FluentValidation;
using IEventAggregator = EDH.Core.Events.Abstractions.IEventAggregator;

namespace EDH.Sales.Application.Services;

public sealed class SaleService : ISaleService
{
    private readonly IEventAggregator _eventAggregator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISaleRepository _saleRepository;
    private readonly CreateSaleDtoValidator _createSaleDtoValidator = new();

    public SaleService(IEventAggregator eventAggregator, IUnitOfWork unitOfWork, ISaleRepository saleRepository)
    {
        _eventAggregator = eventAggregator;
        _unitOfWork = unitOfWork;
        _saleRepository = saleRepository;
    }
    public async Task<IEnumerable<GetInventoryItemsRecordSaleDto>> GetInventoryItemsByNameAsync(string itemName)
    {
        try
        {
            var completionSource = new TaskCompletionSource<IEnumerable<InventoryItem>>();
            
            _eventAggregator.Publish<GetInventoryItemsByNameEvent, GetInventoryItemsByNameEventParameters>(new GetInventoryItemsByNameEventParameters(itemName)
            {
                CompletionSource = completionSource
            });

            var inventoryItems = await completionSource.Task;

            return inventoryItems.Select(item => new GetInventoryItemsRecordSaleDto(item.Id, item.Item.Name, new GetItemRecordSaleDto(item.Item.Id, item.Item.SellingPrice, item.Item.ItemVariableCosts.Sum(vc => vc.Value))));
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<int> CreateSaleAsync(SaleRecordSaleDto saleDto)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            
            var validationResult = await _createSaleDtoValidator.ValidateAsync(saleDto);

            if (!validationResult.IsValid)
            {
                string errorMessages = String.Join(" - ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ValidationException(errorMessages);
            }

            var sale = new Sale
            {
                TotalVariableCosts = saleDto.TotalVariableCosts,
                TotalProfit = saleDto.TotalProfit,
                TotalAdjustment = saleDto.TotalAdjustment,
                TotalValue = saleDto.TotalValue,
                SaleLines = saleDto.SaleLines.Select(sl => new SaleLine
                {
                    ItemId = sl.ItemId,
                    UnitPrice = sl.UnitPrice,
                    Quantity = sl.Quantity,
                    UnitVariableCosts = sl.Costs / sl.Quantity,
                    TotalVariableCosts = sl.Costs,
                    Adjustment = sl.Adjustment,
                    Profit = sl.Profit,
                    Subtotal = sl.Subtotal
                }).ToList()
            };
            
            await _saleRepository.AddAsync(sale);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
            return sale.Id;
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}