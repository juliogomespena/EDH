namespace EDH.Items.Application.DTOs.Request.CreateItemInventory;

public sealed record CreateItemInventoryRequest(int? InitialStock, int? StockAlertThreshold);