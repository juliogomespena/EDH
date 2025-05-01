namespace EDH.Items.Application.DTOs.CreateItem;

public sealed record CreateItemInventoryDto(int? InitialStock, int? StockAlertThreshold);