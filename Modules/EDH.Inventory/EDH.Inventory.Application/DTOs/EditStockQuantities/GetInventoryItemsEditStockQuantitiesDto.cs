namespace EDH.Inventory.Application.DTOs.EditStockQuantities;

public record GetInventoryItemsEditStockQuantitiesDto(int Id, string Name, int Quantity, int? AlertThreshold);