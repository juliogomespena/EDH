using EDH.Items.Application.DTOs.CreateItem;

namespace EDH.Items.Application.Services.Interfaces;

public interface IItemService
{
	Task<int> CreateItemAsync(CreateItemDto createItemDto);
}