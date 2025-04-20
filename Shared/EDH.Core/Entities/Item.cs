namespace EDH.Core.Entities;

public sealed class Item
{
	public int Id { get; set; }
	public required string Name { get; set; }
	public string? Description { get; set; }
	public required decimal SellingPrice { get; set; }
	public int? ItemCategoryId { get; set; }
	public ItemCategory? ItemCategory { get; set; }
	public ICollection<ItemVariableCost> ItemVariableCosts { get; set; } = new List<ItemVariableCost>();
}