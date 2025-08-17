using EDH.Core.Enums;

namespace EDH.Sales.Application.DTOs.Requests.Models;

public sealed record SaleLineModel(int ItemId, string ItemName, decimal UnitPrice, int Quantity, decimal Costs, decimal? Adjustment, decimal Profit, decimal Subtotal, Currency Currency);