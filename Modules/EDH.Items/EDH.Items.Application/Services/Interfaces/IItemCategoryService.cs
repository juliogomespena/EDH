using EDH.Core.Entities;
using EDH.Items.Application.DTOs.CreateItem;

namespace EDH.Items.Application.Services.Interfaces;

public interface IItemCategoryService
{
	Task<IEnumerable<CreateItemCategoryDto>> GetAllCategoriesAsync();

	Task<int> CreateCategoryAsync(CreateItemCategoryDto createItemCategoryDto);
}