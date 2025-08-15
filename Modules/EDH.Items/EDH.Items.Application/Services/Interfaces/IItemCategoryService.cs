using EDH.Core.Common;
using EDH.Items.Application.DTOs.Request.CreateItemCategory;
using EDH.Items.Application.DTOs.Responses.CreateItemCategory;
using EDH.Items.Application.DTOs.Responses.GetAllItemCategories;

namespace EDH.Items.Application.Services.Interfaces;

public interface IItemCategoryService
{
	Task<Result<IEnumerable<GetAllItemCategoriesResponse>>> GetAllItemCategoriesAsync();

	Task<Result<CreateItemCategoryResponse>> CreateItemCategoryAsync(CreateItemCategoryRequest request);
}