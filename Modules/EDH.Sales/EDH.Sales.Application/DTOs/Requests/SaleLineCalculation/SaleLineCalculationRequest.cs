using EDH.Core.Enums;

namespace EDH.Sales.Application.DTOs.Requests.SaleLineCalculation;

public sealed record SaleLineCalculationRequest(decimal UnitPrice, int Quantity, decimal UnitCosts, decimal DiscountSurchargeValue, DiscountSurchargeMode DiscountSurchargeMode, Currency Currency);