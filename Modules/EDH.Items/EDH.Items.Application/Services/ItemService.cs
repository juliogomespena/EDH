using EDH.Core.Entities;
using EDH.Core.Interfaces.IInfrastructure;
using EDH.Core.Interfaces.IInventory;
using EDH.Core.Interfaces.IItems;
using EDH.Items.Application.DTOs.CreateItem;
using EDH.Items.Application.Services.Interfaces;
using EDH.Items.Application.Validators;
using EDH.Items.Application.Validators.CreateItem;
using FluentValidation;

namespace EDH.Items.Application.Services;

public sealed class ItemService : IItemService
{
	private readonly IItemCategoryRepository _itemCategoryRepository;
	private readonly IItemRepository _itemRepository;
	private readonly IInventoryItemRepository _inventoryItemRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly CreateItemDtoValidator _createItemDtoValidator;

	public ItemService(IItemCategoryRepository itemCategoryRepository, IItemRepository itemRepository, IInventoryItemRepository inventoryItemRepository, IUnitOfWork unitOfWork)
	{
		_itemCategoryRepository = itemCategoryRepository;
		_itemRepository = itemRepository;
		_inventoryItemRepository = inventoryItemRepository;
		_unitOfWork = unitOfWork;
		_createItemDtoValidator = new CreateItemDtoValidator();
	}

	public async Task<int> CreateItemAsync(CreateItemDto createItemDto)
	{
		var validationResult = await _createItemDtoValidator.ValidateAsync(createItemDto);

		if (!validationResult.IsValid)
		{
			string errorMessages = String.Join(" - ", validationResult.Errors.Select(e => e.ErrorMessage));
			throw new ValidationException(errorMessages);
		}

		try
		{
			await _unitOfWork.BeginTransactionAsync();

			ItemCategory? category = null;
			switch (createItemDto.ItemCategory?.Id)
			{
				case 0:
					{
						var itemCategoryDtoValidator = new CreateItemCategoryDtoValidator();
						validationResult = await itemCategoryDtoValidator.ValidateAsync(createItemDto.ItemCategory);

						if (!validationResult.IsValid)
						{
							string errorMessages = String.Join(" - ", validationResult.Errors.Select(e => e.ErrorMessage));
							throw new ValidationException(errorMessages);
						}

						var itemCategory = new ItemCategory
						{
							Name = createItemDto.ItemCategory.Name,
							Description = createItemDto.ItemCategory.Description
						};

						await _itemCategoryRepository.AddAsync(itemCategory);
						category = itemCategory;
						break;
					}
				case > 0:
					category = await _itemCategoryRepository.GetByIdAsync(createItemDto.ItemCategory.Id)!;
					break;
			}

			var item = new Item
			{
				Name = createItemDto.Name,
				Description = createItemDto.Description,
				SellingPrice = createItemDto.SellingPrice,
				ItemCategory = category,
				ItemVariableCosts = createItemDto.VariableCosts.Select(vc => new ItemVariableCost
				{
					CostName = vc.Name,
					Value = vc.Value
				}).ToList()
			};

			await _itemRepository.AddAsync(item);
			await _unitOfWork.SaveChangesAsync();

			if (createItemDto.Inventory is not null)
			{
				var itemInventoryDtoValidator = new CreateItemInventoryDtoValidator();
				validationResult = await itemInventoryDtoValidator.ValidateAsync(createItemDto.Inventory);

				if (!validationResult.IsValid)
				{
					string errorMessages = String.Join(" - ", validationResult.Errors.Select(e => e.ErrorMessage));
					throw new ValidationException(errorMessages);
				}

				var inventoryItem = new InventoryItem
				{
					ItemId = item.Id,
					Quantity = createItemDto.Inventory.InitialStock ?? 0,
					AlertThreshold = createItemDto.Inventory.StockAlertThreshold ?? 0,
					LastUpdated = DateTime.Now
				};

				await _inventoryItemRepository.AddAsync(inventoryItem);
				await _unitOfWork.SaveChangesAsync();
			}

			await _unitOfWork.CommitTransactionAsync();

			return item.Id;
		}
		catch (Exception)
		{
			await _unitOfWork.RollbackTransactionAsync();
			throw;
		}
	}
}