using EDH.Core.Enums;
using EDH.Sales.Application.DTOs.Requests.Models;

namespace EDH.Sales.Application.DTOs.Requests.CreateSale;

public sealed record CreateSaleRequest(int Id, decimal TotalVariableCosts, decimal TotalProfit, decimal? TotalAdjustment, decimal TotalValue, IEnumerable<SaleLineModel> SaleLines, Currency Currency);