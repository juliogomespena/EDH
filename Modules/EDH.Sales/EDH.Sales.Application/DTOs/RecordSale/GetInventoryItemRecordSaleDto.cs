namespace EDH.Sales.Application.DTOs.RecordSale;

public sealed record GetInventoryItemRecordSaleDto(int Id, string Name, GetItemRecordSaleDto ItemRecordSale, int Quantity);