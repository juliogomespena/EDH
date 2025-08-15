using EDH.Sales.Application.DTOs.Request.Models;

namespace EDH.Sales.Application.DTOs.Request.SaleTotalCalculation;

public sealed record SaleTotalCalculationRequest(SaleLineModel[] SaleLines);