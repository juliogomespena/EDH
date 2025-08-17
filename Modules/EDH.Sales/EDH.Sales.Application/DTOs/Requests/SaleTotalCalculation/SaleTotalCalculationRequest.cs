using EDH.Sales.Application.DTOs.Requests.Models;

namespace EDH.Sales.Application.DTOs.Requests.SaleTotalCalculation;

public sealed record SaleTotalCalculationRequest(SaleLineModel[] SaleLines);