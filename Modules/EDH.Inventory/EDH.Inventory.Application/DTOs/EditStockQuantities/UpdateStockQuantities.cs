namespace EDH.Inventory.Application.DTOs.EditStockQuantities;

public sealed record UpdateStockQuantities(int Id, string ItemName, int Quantity, int? AlertThreshold);