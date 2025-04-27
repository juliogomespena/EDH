using EDH.Core.Entities;
using EDH.Core.Interfaces.Infrastructure;
using EDH.Core.Interfaces.Items;
using EDH.Items.Application.DTOs;
using EDH.Items.Application.Services.Interfaces;
using EDH.Items.Application.Validators;
using FluentValidation;

namespace EDH.Items.Application.Services;

public sealed class ItemService :IItemService
{
	private readonly IItemCategoryService _itemCategoryService;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IItemRepository _itemRepository;
	private readonly ItemDtoValidator _validator;

	public ItemService(IItemCategoryService itemCategoryService, IUnitOfWork unitOfWork, IItemRepository itemRepository)
	{
		_itemCategoryService = itemCategoryService;
		_unitOfWork = unitOfWork;
		_itemRepository = itemRepository;
		_validator = new ItemDtoValidator();
	}
	public async Task<int> CreateItemAsync(ItemDto itemDto)
	{
		var validationResult = await _validator.ValidateAsync(itemDto);

		if (!validationResult.IsValid)
		{
			string errorMessages = String.Join(" - ", validationResult.Errors.Select(e => e.ErrorMessage));
			throw new ValidationException(errorMessages);
		}

		int? categoryId = null;
		if (itemDto.ItemCategory?.Id == 0)
		{
			categoryId = await _itemCategoryService.CreateCategoryAsync(itemDto.ItemCategory);
		}

		var item = new Item
		{
			Name = itemDto.Name,
			Description = itemDto.Description,
			SellingPrice = itemDto.SellingPrice,
			ItemCategoryId = categoryId,
			ItemVariableCosts = itemDto.VariableCosts.Select(vc => new ItemVariableCost
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