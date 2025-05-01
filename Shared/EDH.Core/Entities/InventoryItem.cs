namespace EDH.Core.Entities;

public sealed class InventoryItem
{
	public int Id { get; set; }
	public int ItemId { get; set; }
	public int Quantity { get; set; }
	public int? AlertThreshold { get; set; }
	public DateTime LastUpdated { get; set; }
	public Item Item { get; set; } = null!;
}