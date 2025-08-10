using EDH.Core.Common;
using EDH.Items.Application.DTOs.CreateItem;

namespace EDH.Items.Application.Services.Interfaces;

public interface IItemCategoryService
{
	Task<Result<IEnumerable<CreateItemCategoryDto>>> GetAllItemCategoriesAsync();

	Task<Result<CreateItemCategoryDto>> CreateItemCategoryAsync(CreateItemCategoryDto createItemCategoryDto);
}