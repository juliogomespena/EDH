using EDH.Items.Application.DTOs.Requests.CreateItemCategory;
using EDH.Items.Application.DTOs.Requests.CreateItemInventory;
using EDH.Items.Application.DTOs.Requests.CreateItemVariableCosts;

namespace EDH.Items.Application.DTOs.Requests.CreateItem;

public sealed record CreateItemRequest (int Id, string Name, string? Description, decimal SellingPrice, CreateItemCategoryRequest? ItemCategory, IEnumerable<CreateItemVariableCostRequest> VariableCosts, CreateItemInventoryRequest? Inventory);