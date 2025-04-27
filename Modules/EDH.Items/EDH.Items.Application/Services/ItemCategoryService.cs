using EDH.Core.Entities;
using EDH.Core.Interfaces.Infrastructure;
using EDH.Core.Interfaces.Items;
using EDH.Items.Application.DTOs;
using EDH.Items.Application.Services.Interfaces;
using EDH.Items.Application.Validators;
using FluentValidation;

namespace EDH.Items.Application.Services;

public sealed class ItemCategoryService : IItemCategoryService
{
	private readonly IItemCategoryRepository _itemCategoryRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ItemCategoryDtoValidator _validator;

	public ItemCategoryService(IItemCategoryRepository itemCategoryRepository, IUnitOfWork unitOfWork)
	{
		_itemCategoryRepository = itemCategoryRepository;
		_unitOfWork = unitOfWork;
		_validator = new ItemCategoryDtoValidator();
	}

	public async Task<IEnumerable<ItemCategoryDto>> GetAllCategoriesAsync()
	{
		var categories = await _itemCategoryRepository.GetAllAsync();

		return categories.Select(c => new ItemCategoryDto(c.Id, c.Name, c.Description));
	}

	public async Task<int> CreateCategoryAsync(ItemCategoryDto itemCategoryDto)
	{
		var validationResult = await _validator.ValidateAsync(itemCategoryDto);

		if (!validationResult.IsValid)
		{
			string errorMessages = String.Join(" - ", validationResult.Errors.Select(e => e.ErrorMessage));
			throw new ValidationException(errorMessages);
		}

		var itemCategory = new ItemCategory
		{
			Name = itemCategoryDto.Name,
			Description = itemCategoryDto.Description
		};

		await _itemCategoryRepository.AddAsync(itemCategory);
		await _unitOfWork.SaveChangesAsync();

		return itemCategory.Id;
	}
}