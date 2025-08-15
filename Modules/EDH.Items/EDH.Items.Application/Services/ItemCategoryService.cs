using EDH.Core.Common;
using EDH.Core.Entities;
using EDH.Core.Interfaces.IInfrastructure;
using EDH.Core.Interfaces.IItems;
using EDH.Items.Application.DTOs.Request.CreateItem;
using EDH.Items.Application.DTOs.Request.CreateItemCategory;
using EDH.Items.Application.DTOs.Responses.CreateItemCategory;
using EDH.Items.Application.DTOs.Responses.GetAllItemCategories;
using EDH.Items.Application.Services.Interfaces;
using EDH.Items.Application.Validators.CreateItem;
using EDH.Items.Application.Validators.CreateItemCategory;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace EDH.Items.Application.Services;

public sealed class ItemCategoryService : IItemCategoryService
{
	private readonly IItemCategoryRepository _itemCategoryRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ILogger<ItemCategoryService> _logger;
	private readonly CreateItemCategoryValidator _validator;

	public ItemCategoryService(IItemCategoryRepository itemCategoryRepository, IUnitOfWork unitOfWork, ILogger<ItemCategoryService> logger)
	{
		_itemCategoryRepository = itemCategoryRepository;
		_unitOfWork = unitOfWork;
		_logger = logger;
		_validator = new CreateItemCategoryValidator();
	}

	public async Task<Result<IEnumerable<GetAllItemCategoriesResponse>>> GetAllItemCategoriesAsync()
	{
		try
		{
			var categories = await _itemCategoryRepository.GetAllAsync();

			return Result<IEnumerable<GetAllItemCategoriesResponse>>.Ok(categories
				.Select(c => new GetAllItemCategoriesResponse(c.Id, c.Name, c.Description)));
		}
		catch (Exception ex)
		{
			_logger.LogCritical(ex, $"Error in {nameof(GetAllItemCategoriesAsync)}.");
			throw;
		}
	}

	public async Task<Result<CreateItemCategoryResponse>> CreateItemCategoryAsync(CreateItemCategoryRequest createItemCategoryRequest)
	{
		try
		{
			var validationResult = await _validator.ValidateAsync(createItemCategoryRequest);

			if (!validationResult.IsValid)
			{
				string[] errorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToArray();
				return Result<CreateItemCategoryResponse>.Fail(errorMessages);
			}

			var itemCategory = new ItemCategory
			{
				Name = createItemCategoryRequest.Name,
				Description = createItemCategoryRequest.Description
			};

			await _itemCategoryRepository.AddAsync(itemCategory);
			await _unitOfWork.SaveChangesAsync();

			return Result<CreateItemCategoryResponse>.Ok(new CreateItemCategoryResponse(itemCategory.Id, itemCategory.Name));
		}
		catch (Exception ex)
		{
			_logger.LogCritical(ex, $"Error in {nameof(CreateItemCategoryAsync)}.");
			throw;
		}
	}
}