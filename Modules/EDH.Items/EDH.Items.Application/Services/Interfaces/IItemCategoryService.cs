using EDH.Core.Common;
using EDH.Items.Application.DTOs.CreateItem;

namespace EDH.Items.Application.Services.Interfaces;

public interface IItemCategoryService
{
	Task<Result<IEnumerable<CreateItemCategory>>> GetAllItemCategoriesAsync();

	Task<Result<CreateItemCategory>> CreateItemCategoryAsync(CreateItemCategory createItemCategory);
}