namespace EDH.Inventory.Application.DTOs.Requests.UpdateStockQuantities;

public sealed record UpdateStockQuantitiesRequest(int Id, string ItemName, int Quantity, int? AlertThreshold);