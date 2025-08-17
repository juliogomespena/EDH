namespace EDH.Inventory.Application.DTOs.Request.UpdateStockQuantities;

public sealed record UpdateStockQuantitiesRequest(int Id, string ItemName, int Quantity, int? AlertThreshold);