using EDH.Core.Common;
using EDH.Core.Entities;
using EDH.Core.Enums;
using EDH.Core.Events.Inventory;
using EDH.Core.Events.Inventory.Parameters;
using EDH.Core.Interfaces.IInfrastructure;
using EDH.Core.Interfaces.IItems;
using EDH.Core.ValueObjects;
using EDH.Items.Application.DTOs.CreateItem;
using EDH.Items.Application.Services.Interfaces;
using EDH.Items.Application.Validators.CreateItem;
using Microsoft.Extensions.Logging;
using IEventAggregator = EDH.Core.Events.Abstractions.IEventAggregator;

namespace EDH.Items.Application.Services;

public sealed class ItemService : IItemService
{
	private readonly IItemCategoryRepository _itemCategoryRepository;
	private readonly IItemRepository _itemRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IEventAggregator _eventAggregator;
	private readonly ILogger<ItemService> _logger;
	private readonly CreateItemValidator _createItemValidator = new();

	public ItemService(IItemCategoryRepository itemCategoryRepository, IItemRepository itemRepository, IUnitOfWork unitOfWork, IEventAggregator eventAggregator, ILogger<ItemService> logger)
	{
		_itemCategoryRepository = itemCategoryRepository;
		_itemRepository = itemRepository;
		_unitOfWork = unitOfWork;
		_eventAggregator = eventAggregator;
		_logger = logger;
	}

	public async Task<Result<CreateItem>> CreateItemAsync(CreateItem createItem)
	{
		try
		{
			var validationResult = await _createItemValidator.ValidateAsync(createItem);

			if (!validationResult.IsValid)
			{
				string[] errorMessages =validationResult.Errors.Select(e => e.ErrorMessage).ToArray();
				return Result<CreateItem>.Fail(errorMessages);
			}
			
			await _unitOfWork.BeginTransactionAsync();

			ItemCategory? category = null;
			switch (createItem.ItemCategory?.Id)
			{
				case 0:
					{
						var itemCategoryDtoValidator = new CreateItemCategoryValidator();
						validationResult = await itemCategoryDtoValidator.ValidateAsync(createItem.ItemCategory);

						if (!validationResult.IsValid)
						{
							string[] errorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToArray();
							return Result<CreateItem>.Fail(errorMessages);
						}

						var itemCategory = new ItemCategory
						{
							Name = createItem.ItemCategory.Name,
							Description = createItem.ItemCategory.Description
						};

						await _itemCategoryRepository.AddAsync(itemCategory);
						category = itemCategory;
						break;
					}
				case > 0:
					category = await _itemCategoryRepository.GetByIdAsync(createItem.ItemCategory.Id);
					break;
			}

			var item = new Item
			{
				Name = createItem.Name,
				Description = createItem.Description,
				SellingPrice = Money.FromAmount(createItem.SellingPrice, Currency.Usd),
				ItemCategory = category,
				ItemVariableCosts = createItem.VariableCosts.Select(vc => new ItemVariableCost
				{
					CostName = vc.Name,
					Value = Money.FromAmount(vc.Value, Currency.Usd),
					Currency = Currency.Usd
				}).ToList(),
				Currency = Currency.Usd
			};

			await _itemRepository.AddAsync(item);
			await _unitOfWork.SaveChangesAsync();

			if (createItem.Inventory is not null)
			{
				var itemInventoryDtoValidator = new CreateItemInventoryValidator();
				validationResult = await itemInventoryDtoValidator.ValidateAsync(createItem.Inventory);

				if (!validationResult.IsValid)
				{
					await _unitOfWork.RollbackTransactionAsync();
					string[] errorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToArray();
					return Result<CreateItem>.Fail(errorMessages);
				}
			}

			var completionSource = new TaskCompletionSource<Result<InventoryItem>>();

			_eventAggregator.Publish<CreateInventoryItemEvent, CreateInventoryItemEventParameters>(new CreateInventoryItemEventParameters(item.Id, createItem.Inventory?.InitialStock, createItem.Inventory?.StockAlertThreshold)
			{
				CompletionSource = completionSource
			});

			await completionSource.Task;

			await _unitOfWork.CommitTransactionAsync();
			return Result<CreateItem>.Ok(createItem with { Id = item.Id });
		}
		catch (Exception ex)
		{
			await _unitOfWork.RollbackTransactionAsync();
			_logger.LogCritical(ex, $"Error in {nameof(CreateItemAsync)}.");
			throw;
		}
	}
}