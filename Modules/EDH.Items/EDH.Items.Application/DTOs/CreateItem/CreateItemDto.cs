namespace EDH.Items.Application.DTOs.CreateItem;

public sealed record CreateItemDto (int Id, string Name, string? Description, decimal SellingPrice, CreateItemCategoryDto? ItemCategory, IEnumerable<CreateItemVariableCostDto> VariableCosts, CreateItemInventoryDto? Inventory);