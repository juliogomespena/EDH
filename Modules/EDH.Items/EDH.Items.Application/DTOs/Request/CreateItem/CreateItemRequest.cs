using EDH.Items.Application.DTOs.Request.CreateItemCategory;
using EDH.Items.Application.DTOs.Request.CreateItemInventory;
using EDH.Items.Application.DTOs.Request.CreateItemVariableCosts;

namespace EDH.Items.Application.DTOs.Request.CreateItem;

public sealed record CreateItemRequest (int Id, string Name, string? Description, decimal SellingPrice, CreateItemCategoryRequest? ItemCategory, IEnumerable<CreateItemVariableCostRequest> VariableCosts, CreateItemInventoryRequest? Inventory);