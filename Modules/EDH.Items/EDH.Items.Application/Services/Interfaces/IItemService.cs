using EDH.Items.Application.DTOs;

namespace EDH.Items.Application.Services.Interfaces;

public interface IItemService
{
	Task<int> CreateItemAsync(CreateItemDto createItemDto);
}