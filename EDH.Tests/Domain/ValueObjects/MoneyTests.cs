using EDH.Core.Enums;
using EDH.Core.Exceptions;
using EDH.Core.ValueObjects;
using FluentAssertions;

namespace EDH.Tests.Domain.ValueObjects;

public sealed class MoneyTests
{
    [Theory]
    [InlineData(Currency.Usd)]
    [InlineData(Currency.Eur)]
    public void Zero_ShouldReturnMoneyWithZeroAmountAndSpecifiedCurrency(Currency currency)
    {
        var result = Money.Zero(currency);

        result.Amount.Should().Be(0);
        result.Currency.Should().Be(currency);   
    }
    
    [Theory]
    [InlineData(87.48, Currency.Usd)]
    [InlineData(-44.15, Currency.Eur)]
    [InlineData(-77.48, Currency.Usd)]
    [InlineData(33.07, Currency.Eur)]
    public void FromAmount_ShouldReturnMoneyWithSpecifiedAmountAndCurrency(decimal amount, Currency currency)
    {
        var result = Money.FromAmount(amount, currency);

        result.Amount.Should().Be(amount);
        result.Currency.Should().Be(currency);   
    }
    
    [Theory]
    [InlineData(45.50, Currency.Usd, 50, Currency.Usd, 95.50)]
    [InlineData(40.50, Currency.Eur, 40, Currency.Eur, 80.50)]
    [InlineData(40, Currency.Usd, -50, Currency.Usd, -10)]
    [InlineData(40.50, Currency.Eur, -50, Currency.Eur, -9.5)]
    public void Add_ShouldReturnCorrectAmountWhenCurrenciesAreTheSame(decimal moneyAmount, Currency moneyCurrency, 
        decimal otherAmount, Currency otherCurrency, decimal expectedResult)
    {
        var money = Money.FromAmount(moneyAmount, moneyCurrency);
        var other = Money.FromAmount(otherAmount, otherCurrency);
        
        var result = money.Add(other);
        
        result.Amount.Should().Be(expectedResult);
        result.Currency.Should().Be(moneyCurrency);  
    }
    
    [Theory]
    [InlineData(45.50, Currency.Usd, 50, Currency.Eur)]
    [InlineData(40.50, Currency.Eur, -50, Currency.Usd)]
    public void Add_ShouldThrowInvalidCurrencyExceptionWhenCurrenciesAreNotTheSame(decimal moneyAmount, Currency moneyCurrency, 
        decimal otherAmount, Currency otherCurrency)
    {
        var money = Money.FromAmount(moneyAmount, moneyCurrency);
        var other = Money.FromAmount(otherAmount, otherCurrency);
        
        Action act = () => money.Add(other);
        
        act.Should().Throw<InvalidCurrencyException>(); 
    }
}