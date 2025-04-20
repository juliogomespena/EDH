namespace EDH.Core.Entities;

public sealed class ItemVariableCost
{
	public int Id { get; set; }
	public required string CostName { get; set; }
	public required decimal Value { get; set; }
	public int ItemId { get; set; }
	public Item Item { get; set; } = null!;
}