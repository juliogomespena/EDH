namespace EDH.Core.Entities;

public sealed class SaleLine
{
    public int Id { get; set; }
    public required int ItemId { get; set; }
    public required decimal UnitPrice { get; set; }
    public required int Quantity { get; set; }
    public required decimal UnitVariableCosts { get; set; }
    public required decimal TotalVariableCosts { get; set; }
    public decimal? Adjustment { get; set; }
    public required decimal Profit { get; set; }
    public required decimal Subtotal { get; set; }
    public int SaleId { get; set; }
    public Item Item { get; set; } = null!;
    public Sale Sale { get; set; } = null!;
}