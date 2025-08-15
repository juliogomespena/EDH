using EDH.Core.Common;
using EDH.Items.Application.DTOs.Request.CreateItem;
using EDH.Items.Application.DTOs.Request.GetProfitMargin;
using EDH.Items.Application.DTOs.Responses.CreateItem;
using EDH.Items.Application.DTOs.Responses.GetProfitMargin;

namespace EDH.Items.Application.Services.Interfaces;

public interface IItemService
{
	Result<GetProfitMarginResponse> GetProfitMargin(GetProfitMarginRequest request);
	
	Task<Result<CreateItemResponse>> CreateItemAsync(CreateItemRequest request);
}