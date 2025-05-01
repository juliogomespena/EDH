namespace EDH.Inventory.Application.DTOs;

public record GetInventoryItemDto(int Id, string Name, int? Quantity, int? AlertThreshold);