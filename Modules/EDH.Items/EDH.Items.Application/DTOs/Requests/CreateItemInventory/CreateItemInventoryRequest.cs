namespace EDH.Items.Application.DTOs.Requests.CreateItemInventory;

public sealed record CreateItemInventoryRequest(int? InitialStock, int? StockAlertThreshold);