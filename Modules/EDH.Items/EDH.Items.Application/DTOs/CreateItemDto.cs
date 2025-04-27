namespace EDH.Items.Application.DTOs;

public sealed record CreateItemDto (int Id, string Name, string? Description, decimal SellingPrice, CreateItemCategoryDto? ItemCategory, IEnumerable<CreateItemVariableCostDto> VariableCosts);

public sealed record CreateItemVariableCostDto (string Name, decimal Value);