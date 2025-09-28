using EDH.Core.Exceptions;
using EDH.Core.ValueObjects;
using FluentAssertions;

namespace EDH.Tests.Domain.ValueObjects;

public sealed class QuantityTests
{
    [Fact]
    public void Zero_ShouldReturnQuantityWithZeroValue()
    {
        var quantity = Quantity.Zero;
        
        quantity.Value.Should().Be(0); 
    }
    
    [Theory]
    [InlineData(10)]
    [InlineData(0)]
    public void FromValue_ShouldReturnQuantityWithCorrectValue(int value)
    {
        var quantity = Quantity.FromValue(value);
        
        quantity.Value.Should().Be(value); 
    }

    [Fact]
    public void FromValue_ShouldBeAbleToHandleExtremeValues()
    {
        var quantity = Quantity.FromValue(Int32.MaxValue);
        
        quantity.Value.Should().Be(Int32.MaxValue);
    }

    [Fact]
    public void FromValue_ShouldThrowInvalidQuantityExceptionWhenValueIsNegative()
    {
        var act = () => Quantity.FromValue(-1);
        
        act.Should().Throw<InvalidQuantityException>();
    }

    [Theory]
    [InlineData(10, 10, 20)]
    [InlineData(0, 10, 10)]
    [InlineData(1, 0, 1)]
    public void Add_ShouldReturnCorrectValue(int quantityOne, int quantityTwo, int expectedResult)
    {
        var quantityOneObject = Quantity.FromValue(quantityOne);
        var quantityTwoObject = Quantity.FromValue(quantityTwo);
        
        var result = quantityOneObject.Add(quantityTwoObject);
        
        result.Value.Should().Be(expectedResult);
    }
    
    [Fact]
    public void Add_ShouldThrowOverflowExceptionWhenResultExceedsMaxValue()
    {
        var quantity1 = Quantity.FromValue(Int32.MaxValue);
        var quantity2 = Quantity.FromValue(1);
    
        var act = () => quantity1.Add(quantity2);
    
        act.Should().Throw<OverflowException>();
    }
    
    [Fact]
    public void Add_ShouldThrowArgumentNullExceptionWhenQuantityIsNull()
    {
        var quantity = Quantity.FromValue(10);
    
        var act = () => quantity.Add(null!);
    
        act.Should().Throw<ArgumentNullException>();
    }
    
    [Theory]
    [InlineData(10, 10, 0)]
    [InlineData(11, 0, 11)]
    [InlineData(12, 3, 9)]
    public void Subtract_ShouldReturnCorrectValue(int quantityOne, int quantityTwo, int expectedResult)
    {
        var quantityOneObject = Quantity.FromValue(quantityOne);
        var quantityTwoObject = Quantity.FromValue(quantityTwo);
        
        var result = quantityOneObject.Subtract(quantityTwoObject);
        
        result.Value.Should().Be(expectedResult);
    }
    
    [Fact]
    public void Subtract_ShouldThrowInvalidQuantityExceptionWhenResultIsNegative()
    {
        var quantityOne = Quantity.FromValue(10);
        var quantityTwo = Quantity.FromValue(11);
        
        var act = () => quantityOne.Subtract(quantityTwo);
        
        act.Should().Throw<InvalidQuantityException>();
    }
    
    [Fact]
    public void Subtract_ShouldThrowArgumentNullExceptionWhenQuantityIsNull()
    {
        var quantity = Quantity.FromValue(10);

        var act = () => quantity.Subtract(null!);

        act.Should().Throw<ArgumentNullException>();
    }


    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    public void IsZero_ShouldReturnCorrectBoolean(int value)
    {
        var quantity = Quantity.FromValue(value);
        
        quantity.IsZero.Should().Be(value == 0);
    }
    
    [Theory]
    [InlineData(1, 2)]
    [InlineData(0, 3)]
    [InlineData(3, 2)]
    [InlineData(1, 0)]
    [InlineData(0, 0)]
    public void IsGreaterThan_ShouldReturnCorrectBoolean(int valueOne, int valueTwo)
    {
        var quantity = Quantity.FromValue(valueOne);
        
        quantity.IsGreaterThan(valueTwo).Should().Be(valueOne > valueTwo);
    }
    
    [Theory]
    [InlineData(1, 2)]
    [InlineData(0, 3)]
    [InlineData(3, 2)]
    [InlineData(1, 0)]
    [InlineData(0, 0)]
    public void IsLessThan_ShouldReturnCorrectBoolean(int valueOne, int valueTwo)
    {
        var quantity = Quantity.FromValue(valueOne);
        
        quantity.IsLessThan(valueTwo).Should().Be(valueOne < valueTwo);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    [InlineData(3)]
    public void ToString_ShouldReturnCorrectString(int value)
    {
        var quantity = Quantity.FromValue(value);
        
        quantity.ToString().Should().Be(value.ToString());
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    [InlineData(3)]
    public void NonNullableImplicitOperator_ShouldReturnCorrectIntValue(int value)
    {
        var quantity = Quantity.FromValue(value);
        
        int result = quantity;
        
        result.Should().Be(value);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    [InlineData(3)]
    public void NullableImplicitOperator_ShouldReturnCorrectIntValue(int value)
    {
        var quantity = Quantity.FromValue(value);
        
        int? result = quantity;
        
        result.Should().Be(value);
    }
    
    [Fact]
    public void NullableImplicitOperator_ShouldReturnNullWhenQuantityIsNull()
    {
        Quantity? quantity = null;
    
        int? result = quantity;
    
        result.Should().BeNull();
    }
}