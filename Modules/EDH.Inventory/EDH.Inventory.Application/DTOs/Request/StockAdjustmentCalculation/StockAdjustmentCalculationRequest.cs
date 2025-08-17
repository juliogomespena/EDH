namespace EDH.Inventory.Application.DTOs.Request.StockAdjustmentCalculation;

public sealed record StockAdjustmentCalculationRequest(int CurrentQuantity, int Adjustment);