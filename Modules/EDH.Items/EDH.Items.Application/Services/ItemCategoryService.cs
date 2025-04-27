using EDH.Core.Entities;
using EDH.Core.Interfaces.IInfrastructure;
using EDH.Core.Interfaces.IItems;
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

	public async Task<IEnumerable<CreateItemCategoryDto>> GetAllCategoriesAsync()
	{
		var categories = await _itemCategoryRepository.GetAllAsync();

		return categories.Select(c => new CreateItemCategoryDto(c.Id, c.Name, c.Description));
	}

	public async Task<int> CreateCategoryAsync(CreateItemCategoryDto createItemCategoryDto)
	{
		var validationResult = await _validator.ValidateAsync(createItemCategoryDto);

		if (!validationResult.IsValid)
		{
			string errorMessages = String.Join(" - ", validationResult.Errors.Select(e => e.ErrorMessage));
			throw new ValidationException(errorMessages);
		}

		var itemCategory = new ItemCategory
		{
			Name = createItemCategoryDto.Name,
			Description = createItemCategoryDto.Description
		};

		await _itemCategoryRepository.AddAsync(itemCategory);
		await _unitOfWork.SaveChangesAsync();

		return itemCategory.Id;
	}
}