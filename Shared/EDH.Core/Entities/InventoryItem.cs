using EDH.Core.ValueObjects;

namespace EDH.Core.Entities;

public sealed class InventoryItem
{
	public int Id { get; set; }
	public required int ItemId { get; set; }
	public required Quantity Quantity { get; set; }
	public Quantity? AlertThreshold { get; set; }
	public DateTime LastUpdated { get; set; }
	public Item Item { get; set; } = null!;
}