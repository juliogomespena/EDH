namespace EDH.Inventory.Application.DTOs;

public sealed record CreateInventoryItemDto(int Id, int ItemId, int Quantity, int AlertThreshold, DateTime LastUpdated);