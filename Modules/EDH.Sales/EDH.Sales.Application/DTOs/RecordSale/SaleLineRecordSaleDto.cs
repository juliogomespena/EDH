namespace EDH.Sales.Application.DTOs.RecordSale;

public sealed record SaleLineRecordSaleDto(int ItemId, string ItemName, decimal UnitPrice, int Quantity, decimal Costs, decimal? Adjustment, decimal Profit, decimal Subtotal);