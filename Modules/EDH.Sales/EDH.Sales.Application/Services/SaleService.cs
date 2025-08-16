using EDH.Core.Common;
using EDH.Core.Entities;
using EDH.Core.Events.Inventory;
using EDH.Core.Events.Inventory.Parameters;
using EDH.Core.Interfaces.IInfrastructure;
using EDH.Core.Interfaces.ISales;
using EDH.Core.ValueObjects;
using EDH.Sales.Application.DTOs.Request.CreateSale;
using EDH.Sales.Application.DTOs.Request.SaleLineCalculation;
using EDH.Sales.Application.DTOs.Request.SaleTotalCalculation;
using EDH.Sales.Application.DTOs.Response.CreateSale;
using EDH.Sales.Application.DTOs.Response.GetInventoryItem;
using EDH.Sales.Application.DTOs.Response.GetInventoryItem.Models;
using EDH.Sales.Application.DTOs.Response.SaleLineCalculation;
using EDH.Sales.Application.DTOs.Response.SaleTotalCalculationResponse;
using EDH.Sales.Application.Services.Interfaces;
using EDH.Sales.Application.Validators.CreateSale;
using EDH.Sales.Core.Services.Interfaces;
using Microsoft.Extensions.Logging;
using IEventAggregator = EDH.Core.Events.Abstractions.IEventAggregator;

namespace EDH.Sales.Application.Services;

public sealed class SaleService : ISaleService
{
    private readonly IEventAggregator _eventAggregator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISaleRepository _saleRepository;
    private readonly ISaleCalculationService _saleCalculationService;
    private readonly ILogger<SaleService> _logger;
    private readonly CreateSaleValidator _createSaleValidator = new();

    public SaleService(IEventAggregator eventAggregator, IUnitOfWork unitOfWork, ISaleRepository saleRepository, ISaleCalculationService saleCalculationService, ILogger<SaleService> logger)
    {
        _eventAggregator = eventAggregator;
        _unitOfWork = unitOfWork;
        _saleRepository = saleRepository;
        _saleCalculationService = saleCalculationService;
        _logger = logger;
    }
    public async Task<Result<IEnumerable<GetInventoryItemResponse>>> GetInventoryItemsByNameAsync(string itemName)
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
                return Result<IEnumerable<GetInventoryItemResponse>>.Ok([]);
            
            var inventoryItems =  inventoryItemsResult.Value?.Select(item => new GetInventoryItemResponse(item.Id, item.Item.Name,
                new ItemModel(item.Item.Id, item.Item.SellingPrice,
                    item.Item.ItemVariableCosts.Sum(vc => vc.Value)), item.Quantity));
                
            return Result<IEnumerable<GetInventoryItemResponse>>.Ok(inventoryItems);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, $"Error in {nameof(GetInventoryItemsByNameAsync)}.");
            throw;
        }
    }

    public Result<SaleLineCalculationResponse> CalculateSaleLine(SaleLineCalculationRequest request)
    {
        try
        {
            var unitPrice = Money.FromAmount(request.UnitPrice, request.Currency);
            var quantity = Quantity.FromValue(request.Quantity);
            var unitCosts = Money.FromAmount(request.UnitCosts, request.Currency);
            var discountSurcharge = request.DiscountSurchargeValue switch
            {
                0 => DiscountSurcharge.None,
                < 0 => DiscountSurcharge.Discount(request.DiscountSurchargeValue, request.DiscountSurchargeMode),
                > 0 => DiscountSurcharge.Surcharge(request.DiscountSurchargeValue, request.DiscountSurchargeMode)
            };

            var result = _saleCalculationService.CalculateLine(unitPrice, quantity, unitCosts, discountSurcharge);

            if (result.IsFailure || result.Value is null)
                return Result<SaleLineCalculationResponse>.Fail(result.Errors.ToArray());

            var saleLineCalculation = result.Value;

            return Result<SaleLineCalculationResponse>.Ok(new SaleLineCalculationResponse(saleLineCalculation.UnitPrice,
                saleLineCalculation.Quantity, saleLineCalculation.Costs, saleLineCalculation.Adjustment,
                saleLineCalculation.Profit, saleLineCalculation.Subtotal, saleLineCalculation.Currency));
        }
        catch (ArgumentException ex)
        {
            _logger.LogCritical(ex, $"Error in {nameof(CalculateSaleLine)}.");
            return Result<SaleLineCalculationResponse>.Fail(ex);
        }
    }

    public Result<SaleTotalCalculationResponse> CalculateSaleTotal(SaleTotalCalculationRequest request)
    {
        try
        {
            var saleLinesResult =  request.SaleLines
                .Select(sl => _saleCalculationService.ReconstructSaleLine(Money.FromAmount(sl.UnitPrice, sl.Currency), 
                    Quantity.FromValue(sl.Quantity), 
                    Money.FromAmount(sl.Costs, sl.Currency), 
                    sl.Adjustment.HasValue
                        ? Money.FromAmount(sl.Adjustment.Value, sl.Currency)
                        : Money.Zero(sl.Currency),
                    Money.FromAmount(sl.Profit, sl.Currency),
                    Money.FromAmount(sl.Subtotal, sl.Currency),
                    sl.Currency))
                .ToArray();
            
            if (saleLinesResult.Any(r => r.IsFailure || r.Value is null))
                return Result<SaleTotalCalculationResponse>.Fail(saleLinesResult.SelectMany(r => r.Errors).ToArray());
            
            var saleLineCalculations = saleLinesResult
                .Select(r => r.Value!).ToArray();

            var result = _saleCalculationService.CalculateTotal(saleLineCalculations);

            if (result.IsFailure || result.Value is null)
                return Result<SaleTotalCalculationResponse>.Fail(result.Errors.ToArray());

            var saleTotalCalculation = result.Value;

            return Result<SaleTotalCalculationResponse>.Ok(new SaleTotalCalculationResponse(saleTotalCalculation.Costs,
                saleTotalCalculation.Profit, saleTotalCalculation.Adjustment, saleTotalCalculation.Total));
        }
        catch (ArgumentException ex)
        {
            _logger.LogCritical(ex, $"Error in {nameof(CalculateSaleTotal)}.");
            return Result<SaleTotalCalculationResponse>.Fail(ex);
        }
    }

    public async Task<Result<CreateSaleResponse>> CreateSaleAsync(CreateSaleRequest request)
    {
        try
        {
            var validationResult = await _createSaleValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                string[] errorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToArray();
                return Result<CreateSaleResponse>.Fail(errorMessages);
            }
            
            var itemQuantityGroups = request.SaleLines.GroupBy(sl => sl.ItemId)
                .Select(g => new { ItemId = g.Key, g.First().ItemName, Quantity = g.Sum(sl => sl.Quantity) })
                .ToArray();
            
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
                    {
                        return Result<CreateSaleResponse>.Fail(
                            $"Error getting inventory for item '{saleLine.ItemName}'.");
                    }

                    case { IsSuccess: true, Value: not null }:
                    {
                        var availabilityResult = _saleCalculationService
                            .HasAvailability(Quantity.FromValue(saleLine.Quantity), inventoryItemResult.Value.Quantity);
                        
                        if (availabilityResult.IsFailure)
                            return Result<CreateSaleResponse>.Fail(availabilityResult.Errors.ToArray());

                        break;
                    }
                }
            }
            
            await _unitOfWork.BeginTransactionAsync();
            
            var sale = new Sale
            {
                TotalVariableCosts = Money.FromAmount(request.TotalVariableCosts, request.Currency),
                TotalProfit = Money.FromAmount(request.TotalProfit, request.Currency),
                TotalAdjustment = request.TotalAdjustment.HasValue
                    ? Money.FromAmount(request.TotalAdjustment.Value, request.Currency)
                    : Money.Zero(request.Currency),
                TotalValue = Money.FromAmount(request.TotalValue, request.Currency),
                SaleLines = request.SaleLines.Select(sl => new SaleLine
                {
                    ItemId = sl.ItemId,
                    UnitPrice = Money.FromAmount(sl.UnitPrice, sl.Currency),
                    Quantity = Quantity.FromValue(sl.Quantity),
                    UnitVariableCosts = Money.FromAmount(sl.Costs / (sl.Quantity == 0 ? 1 : sl.Quantity), sl.Currency),
                    TotalVariableCosts = Money.FromAmount(sl.Costs, sl.Currency),
                    Adjustment = sl.Adjustment.HasValue
                        ? Money.FromAmount(sl.Adjustment.Value, sl.Currency)
                        : Money.Zero(sl.Currency),
                    Profit = Money.FromAmount(sl.Profit, sl.Currency),
                    Subtotal = Money.FromAmount(sl.Subtotal, sl.Currency),
                    Currency = sl.Currency
                }).ToList(),
                Currency = request.Currency
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
                return Result<CreateSaleResponse>.Fail($"Error decreasing inventory for item '{item.ItemName}'.");
            }
            
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
            return Result<CreateSaleResponse>.Ok(new CreateSaleResponse(sale.Id));
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogCritical(ex, $"Error in {nameof(CreateSaleAsync)}.");
            throw;
        }
    }
}