namespace EDH.Inventory.Application.DTOs.Responses.GetInventoryItems;

public sealed record GetInventoryItemsResponse(int Id, string Name, int Quantity, int? AlertThreshold);