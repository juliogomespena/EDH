namespace EDH.Core.Entities;

public sealed class InventoryItem
{
	public int Id { get; set; }
	public required int ItemId { get; set; }
	public required int Quantity { get; set; }
	public int? AlertThreshold { get; set; }
	public DateTime LastUpdated { get; set; }
	public Item Item { get; set; } = null!;
}