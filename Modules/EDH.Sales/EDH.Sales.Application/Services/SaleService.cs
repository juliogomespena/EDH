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
    public async Task<Result<IEnumerable<GetInventoryItemRecordSaleDto>>> GetInventoryItemsByNameAsync(string itemName)
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
                return Result<IEnumerable<GetInventoryItemRecordSaleDto>>.Ok([]);
            
            var inventoryItems =  inventoryItemsResult.Value?.Select(item => new GetInventoryItemRecordSaleDto(item.Id, item.Item.Name,
                new GetItemRecordSaleDto(item.Item.Id, item.Item.SellingPrice,
                    item.Item.ItemVariableCosts.Sum(vc => vc.Value)), item.Quantity));
                
            return Result<IEnumerable<GetInventoryItemRecordSaleDto>>.Ok(inventoryItems);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, $"Error in {nameof(GetInventoryItemsByNameAsync)}.");
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
            
            var itemQuantityGroups = saleDto.SaleLines.GroupBy(sl => sl.ItemId)
                .Select(g => new { ItemId = g.Key, ItemName = g.First().ItemName, Quantity = g.Sum(sl => sl.Quantity) })
                .ToArray();

            var quantityErrors = new List<string>();
            foreach (var saleLine in itemQuantityGroups)
            {
                var completionSource = new TaskCompletionSource<Result<InventoryItem?>>();

                _eventAggregator.Publish<GetInventoryItemsByItemIdEvent, GetInventoryItemByItemIdParameters>(
                    new GetInventoryItemByItemIdParameters(saleLine.ItemId)
                    {
                        CompletionSource = completionSource
                    });
                
                var inventoryItemResult = await completionSource.Task;
                
                switch (inventoryItemResult)
                {
                    case { IsSuccess: true, Value: null}:
                        return Result<SaleRecordSaleDto>.Fail($"Error getting inventory for item '{saleLine.ItemName}'.");
                    
                    case { IsSuccess: true, Value: not null } when
                        inventoryItemResult.Value.Quantity < saleLine.Quantity:
                        quantityErrors.Add($"Not enough '{saleLine.ItemName}' inventory to make sale.");
                        break;
                }
            }
            
            if (quantityErrors.Any())
                return Result<SaleRecordSaleDto>.Fail(quantityErrors.ToArray());
            
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

            foreach (var item in itemQuantityGroups)
            {
                var completionSource = new TaskCompletionSource<Result>();
                _eventAggregator.Publish<DecreaseInventoryItemByItemIdEvent, DecreaseInventoryItemByItemIdParameters>(new DecreaseInventoryItemByItemIdParameters(item.ItemId, item.Quantity)
                {
                    CompletionSource = completionSource
                });
                
                var decreaseInventoryItemResult = await completionSource.Task;

                if (decreaseInventoryItemResult.IsSuccess) continue;
              
                await _unitOfWork.RollbackTransactionAsync();
                return Result<SaleRecordSaleDto>.Fail($"Error decreasing inventory for item '{item.ItemName}'.");
            }
            
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
            return Result<SaleRecordSaleDto>.Ok(saleDto with { Id = sale.Id });
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogCritical(ex, $"Error in {nameof(CreateSaleAsync)}.");
            throw;
        }
    }
}