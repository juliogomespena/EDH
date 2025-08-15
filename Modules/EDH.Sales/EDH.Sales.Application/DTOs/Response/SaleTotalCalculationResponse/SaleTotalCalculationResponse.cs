namespace EDH.Sales.Application.DTOs.Response.SaleTotalCalculationResponse;

public sealed record SaleTotalCalculationResponse(decimal Costs, decimal Profit, decimal Adjustment, decimal Total);