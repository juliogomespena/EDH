namespace EDH.Sales.Application.DTOs.RecordSale;

public sealed record SaleRecordSaleDto(int Id, decimal TotalVariableCosts, decimal TotalProfit, decimal? TotalAdjustment, decimal TotalValue, IEnumerable<SaleLineRecordSaleDto> SaleLines);