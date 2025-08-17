namespace EDH.Inventory.Application.DTOs.Response.GetInventoryItems;

public sealed record GetInventoryItemsResponse(int Id, string Name, int Quantity, int? AlertThreshold);