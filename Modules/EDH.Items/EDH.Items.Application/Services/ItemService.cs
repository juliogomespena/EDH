using EDH.Core.Common;
using EDH.Core.Entities;
using EDH.Core.Enums;
using EDH.Core.Events.Inventory;
using EDH.Core.Events.Inventory.Parameters;
using EDH.Core.Interfaces.IInfrastructure;
using EDH.Core.Interfaces.IItems;
using EDH.Core.ValueObjects;
using EDH.Items.Application.DTOs.Request.CreateItem;
using EDH.Items.Application.DTOs.Request.GetProfitMargin;
using EDH.Items.Application.DTOs.Responses.CreateItem;
using EDH.Items.Application.DTOs.Responses.GetProfitMargin;
using EDH.Items.Application.Services.Interfaces;
using EDH.Items.Application.Validators.CreateItem;
using EDH.Items.Application.Validators.CreateItemCategory;
using EDH.Items.Application.Validators.CreateItemInventory;
using EDH.Items.Core.Services.Interfaces;
using Microsoft.Extensions.Logging;
using IEventAggregator = EDH.Core.Events.Abstractions.IEventAggregator;

namespace EDH.Items.Application.Services;

public sealed class ItemService : IItemService
{
	private readonly IItemCategoryRepository _itemCategoryRepository;
	private readonly IItemRepository _itemRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IEventAggregator _eventAggregator;
	private readonly IProfitMarginCalculationService _profitMarginCalculationService;
	private readonly ILogger<ItemService> _logger;
	private readonly CreateItemValidator _createItemValidator = new();

	public ItemService(IItemCategoryRepository itemCategoryRepository, IItemRepository itemRepository, IUnitOfWork unitOfWork, IEventAggregator eventAggregator, IProfitMarginCalculationService profitMarginCalculationService ,ILogger<ItemService> logger)
	{
		_itemCategoryRepository = itemCategoryRepository;
		_itemRepository = itemRepository;
		_unitOfWork = unitOfWork;
		_eventAggregator = eventAggregator;
		_profitMarginCalculationService = profitMarginCalculationService;
		_logger = logger;
	}

	public Result<GetProfitMarginResponse> GetProfitMargin(GetProfitMarginRequest request)
	{
		try
		{
			var result = _profitMarginCalculationService
				.CalculateProfitMargin(Money.FromAmount(request.Price, request.Currency),
					Money.FromAmount(request.Costs, request.Currency));

			if (result.IsFailure || result.Value is null)
				return Result<GetProfitMarginResponse>.Fail(result.Errors.ToArray());

			var resultProperties = result.Value;

			return Result<GetProfitMarginResponse>.Ok(new GetProfitMarginResponse(resultProperties.Value,
				resultProperties.Percentage, resultProperties.Value.Currency));
		}
		catch (ArgumentException ex)
		{
			_logger.LogCritical(ex, $"Error in {nameof(GetProfitMargin)}.");
			return Result<GetProfitMarginResponse>.Fail(ex);
		}
	}

	public async Task<Result<CreateItemResponse>> CreateItemAsync(CreateItemRequest createItemRequest)
	{
		try
		{
			var validationResult = await _createItemValidator.ValidateAsync(createItemRequest);

			if (!validationResult.IsValid)
			{
				string[] errorMessages =validationResult.Errors.Select(e => e.ErrorMessage).ToArray();
				return Result<CreateItemResponse>.Fail(errorMessages);
			}
			
			await _unitOfWork.BeginTransactionAsync();

			ItemCategory? category = null;
			switch (createItemRequest.ItemCategory?.Id)
			{
				case 0:
					{
						var itemCategoryDtoValidator = new CreateItemCategoryValidator();
						validationResult = await itemCategoryDtoValidator.ValidateAsync(createItemRequest.ItemCategory);

						if (!validationResult.IsValid)
						{
							string[] errorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToArray();
							return Result<CreateItemResponse>.Fail(errorMessages);
						}

						var itemCategory = new ItemCategory
						{
							Name = createItemRequest.ItemCategory.Name,
							Description = createItemRequest.ItemCategory.Description
						};

						await _itemCategoryRepository.AddAsync(itemCategory);
						category = itemCategory;
						break;
					}
				case > 0:
					category = await _itemCategoryRepository.GetByIdAsync(createItemRequest.ItemCategory.Id);
					break;
			}

			var item = new Item
			{
				Name = createItemRequest.Name,
				Description = createItemRequest.Description,
				SellingPrice = Money.FromAmount(createItemRequest.SellingPrice, Currency.Usd),
				ItemCategory = category,
				ItemVariableCosts = createItemRequest.VariableCosts.Select(vc => new ItemVariableCost
				{
					CostName = vc.Name,
					Value = Money.FromAmount(vc.Value, Currency.Usd),
					Currency = Currency.Usd
				}).ToList(),
				Currency = Currency.Usd
			};

			await _itemRepository.AddAsync(item);
			await _unitOfWork.SaveChangesAsync();

			if (createItemRequest.Inventory is not null)
			{
				var itemInventoryDtoValidator = new CreateItemInventoryValidator();
				validationResult = await itemInventoryDtoValidator.ValidateAsync(createItemRequest.Inventory);

				if (!validationResult.IsValid)
				{
					await _unitOfWork.RollbackTransactionAsync();
					string[] errorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToArray();
					return Result<CreateItemResponse>.Fail(errorMessages);
				}
			}

			var completionSource = new TaskCompletionSource<Result<InventoryItem>>();

			_eventAggregator.Publish<CreateInventoryItemEvent, CreateInventoryItemEventParameters>(new CreateInventoryItemEventParameters(item.Id, createItemRequest.Inventory?.InitialStock, createItemRequest.Inventory?.StockAlertThreshold)
			{
				CompletionSource = completionSource
			});

			await completionSource.Task;

			await _unitOfWork.CommitTransactionAsync();
			return Result<CreateItemResponse>.Ok(new CreateItemResponse(item.Id, item.Name));
		}
		catch (Exception ex)
		{
			await _unitOfWork.RollbackTransactionAsync();
			_logger.LogCritical(ex, $"Error in {nameof(CreateItemAsync)}.");
			throw;
		}
	}
}