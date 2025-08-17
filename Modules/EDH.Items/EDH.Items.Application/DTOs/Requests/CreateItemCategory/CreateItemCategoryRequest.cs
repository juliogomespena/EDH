namespace EDH.Items.Application.DTOs.Requests.CreateItemCategory;

public sealed record CreateItemCategoryRequest(int Id, string Name, string? Description);