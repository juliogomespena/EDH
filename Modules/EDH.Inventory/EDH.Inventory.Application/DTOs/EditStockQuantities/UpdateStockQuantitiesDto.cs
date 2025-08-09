namespace EDH.Inventory.Application.DTOs.EditStockQuantities;

public sealed record UpdateStockQuantitiesDto(int Id, string ItemName, int Quantity, int? AlertThreshold);