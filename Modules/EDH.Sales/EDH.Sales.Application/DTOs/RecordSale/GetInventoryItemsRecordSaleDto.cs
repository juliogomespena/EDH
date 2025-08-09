namespace EDH.Sales.Application.DTOs.RecordSale;

public sealed record GetInventoryItemsRecordSaleDto(int Id, string Name, GetItemRecordSaleDto ItemRecordSale);