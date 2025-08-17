namespace EDH.Inventory.Application.DTOs.Requests.StockAdjustmentCalculation;

public sealed record StockAdjustmentCalculationRequest(int CurrentQuantity, int Adjustment);