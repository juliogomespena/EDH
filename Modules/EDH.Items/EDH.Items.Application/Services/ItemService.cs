using EDH.Core.Entities;
using EDH.Core.Interfaces.IInfrastructure;
using EDH.Core.Interfaces.IItems;
using EDH.Items.Application.DTOs;
using EDH.Items.Application.Services.Interfaces;
using EDH.Items.Application.Validators;
using FluentValidation;

namespace EDH.Items.Application.Services;

public sealed class ItemService : IItemService
{
	private readonly IItemCategoryRepository _itemCategoryRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IItemRepository _itemRepository;
	private readonly ItemDtoValidator _validator;

	public ItemService(IItemCategoryRepository itemCategoryRepository, IUnitOfWork unitOfWork, IItemRepository itemRepository)
	{
		_itemCategoryRepository = itemCategoryRepository;
		_unitOfWork = unitOfWork;
		_itemRepository = itemRepository;
		_validator = new ItemDtoValidator();
	}

	public async Task<int> CreateItemAsync(CreateItemDto createItemDto)
	{
		var validationResult = await _validator.ValidateAsync(createItemDto);

		if (!validationResult.IsValid)
		{
			string errorMessages = String.Join(" - ", validationResult.Errors.Select(e => e.ErrorMessage));
			throw new ValidationException(errorMessages);
		}

		ItemCategory? category = null;
		switch (createItemDto.ItemCategory?.Id)
		{
			case 0:
			{
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

		return item.Id;
	}
}