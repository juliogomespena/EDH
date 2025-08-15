namespace EDH.Items.Application.DTOs.CreateItem;

public sealed record CreateItem (int Id, string Name, string? Description, decimal SellingPrice, CreateItemCategory? ItemCategory, IEnumerable<CreateItemVariableCost> VariableCosts, CreateItemInventory? Inventory);