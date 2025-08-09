namespace EDH.Core.Entities;

public sealed class Sale
{
    public int Id { get; set; }
    public required decimal TotalVariableCosts { get; set; }
    public required decimal TotalProfit { get; set; }
    public decimal? TotalAdjustment { get; set; }
    public required decimal TotalValue { get; set; }
    public DateTime Date { get; set; }
    public ICollection<SaleLine> SaleLines { get; set; } = [];
}