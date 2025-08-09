namespace EDH.Sales.Application.DTOs.RecordSale;

public sealed record SaleRecordSaleDto(decimal TotalVariableCosts, decimal TotalProfit, decimal? TotalAdjustment, decimal TotalValue, IEnumerable<SaleLineRecordSaleDto> SaleLines);