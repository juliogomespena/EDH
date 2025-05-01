namespace EDH.Inventory.Application.DTOs.EditStockQuantities;

public record UpdateStockQuantitiesDto(int Id, string ItemName, int Quantity, int? AlertThreshold);