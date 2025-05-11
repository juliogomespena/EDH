namespace EDH.Core.Entities;

public sealed class SaleLine
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal UnitVariableCosts { get; set; }
    public decimal TotalVariableCosts { get; set; }
    public decimal? Adjustment { get; set; }
    public decimal Profit { get; set; }
    public decimal Subtotal { get; set; }
    public Item Item { get; set; } = null!;
}