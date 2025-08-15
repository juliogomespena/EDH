using EDH.Core.Common;
using EDH.Items.Application.DTOs.CreateItem;

namespace EDH.Items.Application.Services.Interfaces;

public interface IItemService
{
	Task<Result<CreateItem>> CreateItemAsync(CreateItem createItem);
}