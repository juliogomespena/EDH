using EDH.Sales.Application.DTOs.Response.GetInventoryItem.Models;

namespace EDH.Sales.Application.DTOs.Response.GetInventoryItem;

public sealed record GetInventoryItemResponse(int Id, string Name, ItemModel ItemModel, int Quantity);