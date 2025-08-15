namespace EDH.Inventory.Application.DTOs.EditStockQuantities;

public sealed record GetInventoryItems(int Id, string Name, int Quantity, int? AlertThreshold);