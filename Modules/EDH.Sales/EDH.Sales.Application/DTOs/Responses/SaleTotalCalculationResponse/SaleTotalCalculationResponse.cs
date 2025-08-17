namespace EDH.Sales.Application.DTOs.Responses.SaleTotalCalculationResponse;

public sealed record SaleTotalCalculationResponse(decimal Costs, decimal Profit, decimal Adjustment, decimal Total);