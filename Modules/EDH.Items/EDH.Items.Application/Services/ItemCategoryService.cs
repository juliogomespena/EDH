using EDH.Core.Entities;
using EDH.Core.Interfaces.IInfrastructure;
using EDH.Core.Interfaces.IItems;
using EDH.Items.Application.DTOs.CreateItem;
using EDH.Items.Application.Services.Interfaces;
using EDH.Items.Application.Validators.CreateItem;
using FluentValidation;

namespace EDH.Items.Application.Services;

public sealed class ItemCategoryService : IItemCategoryService
{
	private readonly IItemCategoryRepository _itemCategoryRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly CreateItemCategoryDtoValidator _validator;

	public ItemCategoryService(IItemCategoryRepository itemCategoryRepository, IUnitOfWork unitOfWork)
	{
		_itemCategoryRepository = itemCategoryRepository;
		_unitOfWork = unitOfWork;
		_validator = new CreateItemCategoryDtoValidator();
	}

	public async Task<IEnumerable<CreateItemCategoryDto>> GetAllCategoriesAsync()
	{
		try
		{
			var categories = await _itemCategoryRepository.GetAllAsync();

			return categories.Select(c => new CreateItemCategoryDto(c.Id, c.Name, c.Description));
		}
		catch (Exception)
		{
			throw;
		}
	}

	public async Task<int> CreateCategoryAsync(CreateItemCategoryDto createItemCategoryDto)
	{
		try
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
		catch (Exception)
		{
			throw;
		}
	}
}