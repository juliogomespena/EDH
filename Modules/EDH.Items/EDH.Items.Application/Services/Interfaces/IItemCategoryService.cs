using EDH.Core.Entities;
using EDH.Items.Application.DTOs;

namespace EDH.Items.Application.Services.Interfaces;

public interface IItemCategoryService
{
	Task<IEnumerable<ItemCategoryDto>> GetAllCategoriesAsync();

	Task<int> CreateCategoryAsync(ItemCategoryDto itemCategoryDto);
}