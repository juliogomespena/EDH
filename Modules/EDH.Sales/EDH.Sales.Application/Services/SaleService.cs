using EDH.Core.Common;
using EDH.Core.Entities;
using EDH.Core.Events.Inventory;
using EDH.Core.Events.Inventory.Parameters;
using EDH.Core.Interfaces.IInfrastructure;
using EDH.Core.Interfaces.ISales;
using EDH.Sales.Application.DTOs.RecordSale;
using EDH.Sales.Application.Services.Interfaces;
using EDH.Sales.Application.Validators.CreateSale;
using FluentValidation;
using Microsoft.Extensions.Logging;
using IEventAggregator = EDH.Core.Events.Abstractions.IEventAggregator;

namespace EDH.Sales.Application.Services;

public sealed class SaleService : ISaleService
{
    private readonly IEventAggregator _eventAggregator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISaleRepository _saleRepository;
    private readonly ILogger<SaleService> _logger;
    private readonly CreateSaleDtoValidator _createSaleDtoValidator = new();

    public SaleService(IEventAggregator eventAggregator, IUnitOfWork unitOfWork, ISaleRepository saleRepository, ILogger<SaleService> logger)
    {
        _eventAggregator = eventAggregator;
        _unitOfWork = unitOfWork;
        _saleRepository = saleRepository;
        _logger = logger;
    }
    public async Task<Result<IEnumerable<GetInventoryItemsRecordSaleDto>>> GetInventoryItemsByNameAsync(string itemName)
    {
        try
        {
            var completionSource = new TaskCompletionSource<Result<IEnumerable<InventoryItem>>>();
            
            _eventAggregator.Publish<GetInventoryItemsByNameEvent, GetInventoryItemsByNameEventParameters>(new GetInventoryItemsByNameEventParameters(itemName)
            {
                CompletionSource = completionSource
            });

            var inventoryItemsResult = await completionSource.Task;

            if (inventoryItemsResult.IsFailure) 
                return Result<IEnumerable<GetInventoryItemsRecordSaleDto>>.Ok([]);
            
            var inventoryItems =  inventoryItemsResult.Value.Select(item => new GetInventoryItemsRecordSaleDto(item.Id,
                item.Item.Name,
                new GetItemRecordSaleDto(item.Item.Id, item.Item.SellingPrice,
                    item.Item.ItemVariableCosts.Sum(vc => vc.Value))));
                
            return Result<IEnumerable<GetInventoryItemsRecordSaleDto>>.Ok(inventoryItems);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error getting inventory items by name.");
            throw;
        }
    }

    public async Task<Result<SaleRecordSaleDto>> CreateSaleAsync(SaleRecordSaleDto saleDto)
    {
        try
        {
            var validationResult = await _createSaleDtoValidator.ValidateAsync(saleDto);

            if (!validationResult.IsValid)
            {
                string[] errorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToArray();
                return Result<SaleRecordSaleDto>.Fail(errorMessages);
            }
            
            await _unitOfWork.BeginTransactionAsync();

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
                    UnitVariableCosts = sl.Costs / (sl.Quantity == 0 ? 1 : sl.Quantity),
                    TotalVariableCosts = sl.Costs,
                    Adjustment = sl.Adjustment,
                    Profit = sl.Profit,
                    Subtotal = sl.Subtotal
                }).ToList()
            };
            
            await _saleRepository.AddAsync(sale);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
            return Result<SaleRecordSaleDto>.Ok(saleDto with { Id = sale.Id });
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogCritical(ex, "Error creating sale.");
            throw;
        }
    }
}