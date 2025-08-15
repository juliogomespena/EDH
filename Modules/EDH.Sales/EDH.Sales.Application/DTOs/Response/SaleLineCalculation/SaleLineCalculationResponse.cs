using EDH.Core.Enums;

namespace EDH.Sales.Application.DTOs.Response.SaleLineCalculation;

public sealed record SaleLineCalculationResponse(decimal UnitPrice, int Quantity, decimal Costs, decimal Adjustment, decimal Profit, decimal Subtotal, Currency Currency);