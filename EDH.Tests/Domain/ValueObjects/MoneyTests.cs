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
    
    [Fact]
    public void FromAmount_ShouldHandleExtremeDecimalValues()
    {
        var maxMoney = Money.FromAmount(Decimal.MaxValue, Currency.Usd);
        var minMoney = Money.FromAmount(Decimal.MinValue, Currency.Eur);

        maxMoney.Amount.Should().Be(Decimal.MaxValue);
        maxMoney.Currency.Should().Be(Currency.Usd);
        
        minMoney.Amount.Should().Be(Decimal.MinValue);
        minMoney.Currency.Should().Be(Currency.Eur);
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
    
    [Theory]
    [InlineData(45.50, Currency.Usd, 5, Currency.Usd, 40.5)]
    [InlineData(40.50, Currency.Eur, -40, Currency.Eur, 80.50)]
    [InlineData(40, Currency.Usd, 50, Currency.Usd, -10)]
    [InlineData(-40.50, Currency.Eur, 50, Currency.Eur, -90.5)]
    [InlineData(-45.5, Currency.Usd, -50, Currency.Usd, 4.5)]
    public void Subtract_ShouldReturnCorrectAmountWhenCurrenciesAreTheSame(decimal moneyAmount, Currency moneyCurrency, 
        decimal otherAmount, Currency otherCurrency, decimal expectedResult)
    {
        var money = Money.FromAmount(moneyAmount, moneyCurrency);
        var other = Money.FromAmount(otherAmount, otherCurrency);
        
        var result = money.Subtract(other);
        
        result.Amount.Should().Be(expectedResult);
        result.Currency.Should().Be(moneyCurrency);  
    }
    
    [Theory]
    [InlineData(45.50, Currency.Usd, 50, Currency.Eur)]
    [InlineData(40.50, Currency.Eur, -50, Currency.Usd)]
    public void Subtract_ShouldThrowInvalidCurrencyExceptionWhenCurrenciesAreNotTheSame(decimal moneyAmount, Currency moneyCurrency, 
        decimal otherAmount, Currency otherCurrency)
    {
        var money = Money.FromAmount(moneyAmount, moneyCurrency);
        var other = Money.FromAmount(otherAmount, otherCurrency);
        
        Action act = () => money.Subtract(other);
        
        act.Should().Throw<InvalidCurrencyException>();
    }
    
    [Theory]
    [InlineData(11.5, Currency.Usd, 3, 34.5)]
    [InlineData(889.58, Currency.Eur, -1, -889.58)]
    [InlineData(-22.5, Currency.Usd, 5, -112.5)]
    [InlineData(-13.89, Currency.Eur, -6.25, 86.8125)]
    [InlineData(2.13, Currency.Usd, -7, -14.91)]
    [InlineData(50, Currency.Usd, 0, 0)]
    [InlineData(0, Currency.Usd, 784, 0)]
    public void MultiplyBy_ShouldReturnMoneyWithCorrectAmount(decimal moneyAmount, Currency moneyCurrency, decimal value, decimal expectedResult)
    {
        var money = Money.FromAmount(moneyAmount, moneyCurrency);
        
        var result = money.MultiplyBy(value);
        
        result.Amount.Should().Be(expectedResult);
        result.Currency.Should().Be(moneyCurrency);  
    }
    
    [Theory]
    [InlineData(11.5, Currency.Usd, 100, 23)]
    [InlineData(889.58, Currency.Eur, -50, 444.79)]
    [InlineData(-22.5, Currency.Usd, 241.58, -76.8555)]
    [InlineData(-13.89, Currency.Eur, -636.36, 74.500404)]
    [InlineData(2.13, Currency.Usd, -7, 1.9809)]
    public void ApplyPercentage_ShouldReturnMoneyWithCorrectAmount(decimal moneyAmount, Currency moneyCurrency, decimal value, decimal expectedResult)
    {
        var money = Money.FromAmount(moneyAmount, moneyCurrency);
        
        var result = money.ApplyPercentage(value);
        
        result.Amount.Should().Be(expectedResult);
        result.Currency.Should().Be(moneyCurrency);  
    }
    
    [Theory]
    [InlineData(11.5, Currency.Usd, 100, 11.5)]
    [InlineData(889.58, Currency.Eur, 23, 204.6034)]
    [InlineData(-22.5, Currency.Usd, 200, -45)]
    [InlineData(-13.89, Currency.Eur, -200, 27.78)]
    [InlineData(2.13, Currency.Usd, -7, -0.1491)]
    public void GetPercentageCorrespondingValue_ShouldReturnMoneyWithCorrectAmount(decimal moneyAmount, Currency moneyCurrency, decimal value, decimal expectedResult)
    {
        var money = Money.FromAmount(moneyAmount, moneyCurrency);
        
        var result = money.GetPercentageCorrespondingValue(value);
        
        result.Amount.Should().Be(expectedResult);
        result.Currency.Should().Be(moneyCurrency);  
    }
    
    [Theory]
    [InlineData(11.5, Currency.Usd)]
    [InlineData(889.58, Currency.Eur)]
    [InlineData(-22.5, Currency.Usd)]
    [InlineData(-13, Currency.Eur)]
    [InlineData(2.13, Currency.Usd)]
    public void ToString_ShouldReturnStringWithCorrectFormating(decimal moneyAmount, Currency moneyCurrency)
    {
        var money = Money.FromAmount(moneyAmount, moneyCurrency);
        
        string result = money.ToString();
        
        result.Should().Be($"{money.Currency.ToString()} {money.Amount:N2}");  
    }
    
    [Theory]
    [InlineData(11.5, Currency.Usd, "C2")]
    [InlineData(889.58, Currency.Eur, "N2")]
    [InlineData(100, Currency.Usd, "F4")]
    [InlineData(1234.5678, Currency.Eur, "C")]
    [InlineData(0, Currency.Usd, "P")]

    public void ToStringWithFormat_ShouldReturnStringWithCorrectFormating(decimal moneyAmount, Currency moneyCurrency, string format)
    {
        var money = Money.FromAmount(moneyAmount, moneyCurrency);
        
        string result = money.ToString(format);
        
        result.Should().Be($"{money.Amount.ToString(format)}");  
    }
    
    [Theory]
    [InlineData(45.50, Currency.Usd)]
    [InlineData(0, Currency.Eur)]
    [InlineData(-25.75, Currency.Usd)]
    public void ImplicitOperator_ShouldReturnAmountWhenConvertingToDecimal(decimal amount, Currency currency)
    {
        var money = Money.FromAmount(amount, currency);
        
        decimal result = money;
        
        result.Should().Be(amount);
    }
}