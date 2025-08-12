using EDH.Core.Common;
using EDH.Core.Entities;
using EDH.Core.Interfaces.IInfrastructure;
using EDH.Core.Interfaces.IItems;
using EDH.Items.Application.DTOs.CreateItem;
using EDH.Items.Application.Services.Interfaces;
using EDH.Items.Application.Validators.CreateItem;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace EDH.Items.Application.Services;

public sealed class ItemCategoryService : IItemCategoryService
{
	private readonly IItemCategoryRepository _itemCategoryRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ILogger<ItemCategoryService> _logger;
	private readonly CreateItemCategoryDtoValidator _validator;

	public ItemCategoryService(IItemCategoryRepository itemCategoryRepository, IUnitOfWork unitOfWork, ILogger<ItemCategoryService> logger)
	{
		_itemCategoryRepository = itemCategoryRepository;
		_unitOfWork = unitOfWork;
		_logger = logger;
		_validator = new CreateItemCategoryDtoValidator();
	}

	public async Task<Result<IEnumerable<CreateItemCategoryDto>>> GetAllItemCategoriesAsync()
	{
		try
		{
			var categories = await _itemCategoryRepository.GetAllAsync();

			return Result<IEnumerable<CreateItemCategoryDto>>.Ok(categories
				.Select(c => new CreateItemCategoryDto(c.Id, c.Name, c.Description)));
		}
		catch (Exception ex)
		{
			_logger.LogCritical(ex, $"Error in {nameof(GetAllItemCategoriesAsync)}.");
			throw;
		}
	}

	public async Task<Result<CreateItemCategoryDto>> CreateItemCategoryAsync(CreateItemCategoryDto createItemCategoryDto)
	{
		try
		{
			var validationResult = await _validator.ValidateAsync(createItemCategoryDto);

			if (!validationResult.IsValid)
			{
				string[] errorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToArray();
				return Result<CreateItemCategoryDto>.Fail(errorMessages);
			}

			var itemCategory = new ItemCategory
			{
				Name = createItemCategoryDto.Name,
				Description = createItemCategoryDto.Description
			};

			await _itemCategoryRepository.AddAsync(itemCategory);
			await _unitOfWork.SaveChangesAsync();

			return Result<CreateItemCategoryDto>.Ok(new CreateItemCategoryDto(itemCategory.Id, itemCategory.Name, itemCategory.Description));
		}
		catch (Exception ex)
		{
			_logger.LogCritical(ex, $"Error in {nameof(CreateItemCategoryAsync)}.");
			throw;
		}
	}
}