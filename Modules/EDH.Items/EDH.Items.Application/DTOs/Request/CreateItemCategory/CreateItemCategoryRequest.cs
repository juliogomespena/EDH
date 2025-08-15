namespace EDH.Items.Application.DTOs.Request.CreateItemCategory;

public sealed record CreateItemCategoryRequest(int Id, string Name, string? Description);