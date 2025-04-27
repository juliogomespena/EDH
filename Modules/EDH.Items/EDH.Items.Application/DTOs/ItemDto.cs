namespace EDH.Items.Application.DTOs;

public record ItemDto (string Name, string? Description, decimal SellingPrice, string? Category, IEnumerable<ItemVariableCostDto> VariableCosts);

public record ItemVariableCostDto (string Name, decimal Value);