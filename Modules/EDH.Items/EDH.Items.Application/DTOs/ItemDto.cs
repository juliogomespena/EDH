namespace EDH.Items.Application.DTOs;

public sealed record ItemDto (int Id, string Name, string? Description, decimal SellingPrice, ItemCategoryDto? ItemCategory, IEnumerable<ItemVariableCostDto> VariableCosts);

public sealed record ItemVariableCostDto (string Name, decimal Value);