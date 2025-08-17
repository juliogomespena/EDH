using EDH.Sales.Application.DTOs.Responses.GetInventoryItem.Models;

namespace EDH.Sales.Application.DTOs.Responses.GetInventoryItem;

public sealed record GetInventoryItemResponse(int Id, string Name, ItemModel ItemModel, int Quantity);