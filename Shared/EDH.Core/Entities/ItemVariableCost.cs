using EDH.Core.Enums;
using EDH.Core.Exceptions;
using EDH.Core.ValueObjects;

namespace EDH.Core.Entities;

public sealed class ItemVariableCost
{
	public int Id { get; set; }
	
	public required string CostName { get; set; }
	
	
	private decimal _valueAmount;
	public required Money Value 
	{ 
		get => Money.FromAmount(_valueAmount, Currency);
		set
		{
			if (value.Currency != Currency)
				throw new InvalidCurrencyException(Currency, value.Currency);
			
			_valueAmount = value.Amount;
		}
	}

	public required Currency Currency { get; set; }
	
	public int ItemId { get; set; }
	
	public Item Item { get; set; } = null!;
}