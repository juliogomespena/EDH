using EDH.Core.Enums;
using EDH.Core.Exceptions;
using EDH.Core.ValueObjects;
using FluentAssertions;

namespace EDH.Tests.Domain.ValueObjects;

public sealed class DiscountSurchargeTests
{
    [Fact]
    public void None_ShouldReturnZeroValueWithCorrectProperties()
    {
        var result = DiscountSurcharge.None;

        result.Should().NotBeNull();
        result.Value.Should().Be(0);
        result.Type.Should().Be(AdjustmentType.None);
        result.Mode.Should().Be(DiscountSurchargeMode.Money);
    }
    
    [Theory]
    [InlineData(-25.52, DiscountSurchargeMode.Money)]
    [InlineData(-100, DiscountSurchargeMode.Money)]
    [InlineData(0, DiscountSurchargeMode.Money)]
    [InlineData(-56.668714, DiscountSurchargeMode.Percentage)]
    [InlineData(-100, DiscountSurchargeMode.Percentage)]
    [InlineData(0, DiscountSurchargeMode.Percentage)]
    public void Discount_WithNegativeValue_ShouldCreateDiscountCorrectly(decimal value, DiscountSurchargeMode mode)
    {
        var result = DiscountSurcharge.Discount(value, mode);
        
        result.Should().NotBeNull();
        result.Value.Should().Be(value);
        result.Type.Should().Be(AdjustmentType.Discount);
        result.Mode.Should().Be(mode);
    }
    
    [Theory]
    [InlineData(18.88, DiscountSurchargeMode.Money)]
    [InlineData(100, DiscountSurchargeMode.Money)]
    [InlineData(26.668714, DiscountSurchargeMode.Percentage)]
    [InlineData(100, DiscountSurchargeMode.Percentage)]
    public void Discount_WithPositiveValue_ShouldThrowException(decimal value, DiscountSurchargeMode mode)
    {
        Action act = () => DiscountSurcharge.Discount(value, mode);

        act.Should().Throw<InvalidDiscountException>();
    }
    
    [Theory]
    [InlineData(55.52, DiscountSurchargeMode.Money)]
    [InlineData(100, DiscountSurchargeMode.Money)]
    [InlineData(0, DiscountSurchargeMode.Money)]
    [InlineData(55.668714, DiscountSurchargeMode.Percentage)]
    [InlineData(100, DiscountSurchargeMode.Percentage)]
    [InlineData(0, DiscountSurchargeMode.Percentage)]
    public void Surcharge_WithPositiveValue_ShouldCreateSurchargeCorrectly(decimal value, DiscountSurchargeMode mode)
    {
        var result = DiscountSurcharge.Surcharge(value, mode);
        
        result.Should().NotBeNull();
        result.Value.Should().Be(value);
        result.Type.Should().Be(AdjustmentType.Surcharge);
        result.Mode.Should().Be(mode);
    }

    [Theory]
    [InlineData(-0.52, DiscountSurchargeMode.Money)]
    [InlineData(-100, DiscountSurchargeMode.Money)]
    [InlineData(-55.668714, DiscountSurchargeMode.Percentage)]
    [InlineData(-100, DiscountSurchargeMode.Percentage)]
    public void Surcharge_WithNegativeValue_ShouldThrowException(decimal value, DiscountSurchargeMode mode)
    {
        Action act = () => DiscountSurcharge.Surcharge(value, mode);
        
        act.Should().Throw<InvalidSurchargeException>();   
    }

    [Theory]
    [InlineData(100, 10, DiscountSurchargeMode.Money, 110, Currency.Usd)]
    [InlineData(100, -10, DiscountSurchargeMode.Money, 90, Currency.Usd)]
    [InlineData(100, 0, DiscountSurchargeMode.Money, 100, Currency.Usd)]
    public void Apply_WithMoneyMode_ShouldAddDiscountSurchargeValueToMoney(decimal originalAmount, decimal adjustmentValue, DiscountSurchargeMode mode,
        decimal expectedResult, Currency currency)
    {
        var money = Money.FromAmount(originalAmount, currency);
        var discountSurcharge = adjustmentValue switch
        {
            0 => DiscountSurcharge.None,
            > 0 => DiscountSurcharge.Surcharge(adjustmentValue, mode),
            < 0 => DiscountSurcharge.Discount(adjustmentValue, mode)
        };
        
        var result = discountSurcharge.Apply(money);
        
        result.Should().Be(Money.FromAmount(expectedResult, currency));   
    }
    
    [Theory]
    [InlineData(100, 10, DiscountSurchargeMode.Percentage, 110, Currency.Usd)]
    [InlineData(100, -10, DiscountSurchargeMode.Percentage, 90, Currency.Usd)]
    [InlineData(100, 0, DiscountSurchargeMode.Percentage, 100, Currency.Usd)]
    public void Apply_WithPercentageMode_ShouldAddDiscountSurchargePercentageValueToMoney(decimal originalAmount, decimal adjustmentValue, DiscountSurchargeMode mode,
        decimal expectedResult, Currency currency)
    {
        var money = Money.FromAmount(originalAmount, currency);
        var discountSurcharge = adjustmentValue switch
        {
            0 => DiscountSurcharge.None,
            > 0 => DiscountSurcharge.Surcharge(adjustmentValue, mode),
            < 0 => DiscountSurcharge.Discount(adjustmentValue, mode)
        };
        
        var result = discountSurcharge.Apply(money);
        
        result.Should().Be(Money.FromAmount(expectedResult, currency));   
    }
    
    [Theory]
    [InlineData(-10, 50, -10, Currency.Usd)]
    [InlineData(0, 100, 0, Currency.Usd)]
    public void DiscountSurchargeValue_DiscountWithMoneyMode_ShouldReturnCorrectValue(decimal discountValue, decimal originalAmount, decimal expectedAmount, Currency currency)
    {
        var discount = DiscountSurcharge.Discount(discountValue, DiscountSurchargeMode.Money);
        var money = Money.FromAmount(originalAmount, currency);
        
        var result = discount.DiscountSurchargeValue(money);
        
        result.Should().Be(Money.FromAmount(expectedAmount, currency));  
    }
    
    [Theory]
    [InlineData(-10, 50, -5, Currency.Usd)]
    [InlineData(0, 100, 0, Currency.Usd)]
    public void DiscountSurchargeValue_DiscountWithPercentageMode_ShouldReturnCorrectValue(decimal discountValue, decimal originalAmount, decimal expectedAmount, Currency currency)
    {
        var discount = DiscountSurcharge.Discount(discountValue, DiscountSurchargeMode.Percentage);
        var money = Money.FromAmount(originalAmount, currency);
        
        var result = discount.DiscountSurchargeValue(money);
        
        result.Should().Be(Money.FromAmount(expectedAmount, currency));  
    }
    
    [Theory]
    [InlineData(10, 50, 10, Currency.Usd)]
    [InlineData(0, 100, 0, Currency.Usd)]
    public void DiscountSurchargeValue_SurchargeWithMoneyMode_ShouldReturnCorrectValue(decimal surchargeValue, decimal originalAmount, decimal expectedAmount, Currency currency)
    {
        var surcharge = DiscountSurcharge.Surcharge(surchargeValue, DiscountSurchargeMode.Money);
        var money = Money.FromAmount(originalAmount, currency);
        
        var result = surcharge.DiscountSurchargeValue(money);
        
        result.Should().Be(Money.FromAmount(expectedAmount, currency));  
    }
    
    [Theory]
    [InlineData(10, 50, 5, Currency.Usd)]
    [InlineData(0, 100, 0, Currency.Usd)]
    public void DiscountSurchargeValue_SurchargeWithPercentageMode_ShouldReturnCorrectValue(decimal surchargeValue, decimal originalAmount, decimal expectedAmount, Currency currency)
    {
        var surcharge = DiscountSurcharge.Surcharge(surchargeValue, DiscountSurchargeMode.Percentage);
        var money = Money.FromAmount(originalAmount, currency);
        
        var result = surcharge.DiscountSurchargeValue(money);
        
        result.Should().Be(Money.FromAmount(expectedAmount, currency));  
    }

    [Fact]
    public void DiscountSurcharge_DiscountProperties_ShouldBeRequired()
    {
        var discount = DiscountSurcharge.Discount(-10, DiscountSurchargeMode.Money);
        
        discount.Value.Should().NotBe(0);  
        discount.Type.Should().NotBe(default);  
        discount.Mode.Should().NotBe(default);  
    }
    
    [Fact]
    public void DiscountSurcharge_SurchargeProperties_ShouldBeRequired()
    {
        var surcharge = DiscountSurcharge.Surcharge(10, DiscountSurchargeMode.Percentage);
        
        surcharge.Value.Should().NotBe(0);  
        surcharge.Type.Should().NotBe(default);  
        surcharge.Mode.Should().NotBe(default);  
    }
}