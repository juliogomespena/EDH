namespace EDH.Inventory.Application.DTOs.EditStockQuantities;

public sealed record GetInventoryItemsEditStockQuantitiesDto(int Id, string Name, int Quantity, int? AlertThreshold);