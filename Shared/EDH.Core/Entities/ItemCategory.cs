namespace EDH.Core.Entities;

public sealed class ItemCategory
{
	public int Id { get; set; }
	public required string Name { get; set; }
	public string? Description { get; set; }
	public ICollection<Item> Items { get; set; } = new List<Item>();
}