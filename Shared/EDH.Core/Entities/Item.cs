using EDH.Core.Enums;
using EDH.Core.Exceptions;
using EDH.Core.ValueObjects;

namespace EDH.Core.Entities;

public sealed class Item
{
	public int Id { get; set; }
	
	public required string Name { get; set; }
	
	public string? Description { get; set; }

	private decimal _sellingPriceAmount;
	public required Money SellingPrice
	{
		get => Money.FromAmount(_sellingPriceAmount, Currency);
		set
		{
			if (value.Currency != Currency)
				throw new InvalidCurrencyException(Currency, value.Currency);
			
			_sellingPriceAmount = value.Amount;
		}
	}

	public Currency Currency { get; set; }
	
	public int? ItemCategoryId { get; set; }
	
	public ItemCategory? ItemCategory { get; set; }
	
	public InventoryItem? Inventory { get; set; }
	
	public ICollection<ItemVariableCost> ItemVariableCosts { get; set; } = [];
	
	public ICollection<SaleLine> SaleLines { get; set; } = [];
}