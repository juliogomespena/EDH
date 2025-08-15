using EDH.Core.Enums;

namespace EDH.Core.Exceptions;

public sealed class InvalidCurrencyException : InvalidOperationException
{
    public InvalidCurrencyException(Currency expectedCurrency, Currency actualCurrency) : base($"Actual currency \"{actualCurrency.ToString()}\" is not valid. Expected \"{expectedCurrency.ToString()}\".")
    {       
        ExpectedCurrency = expectedCurrency;
        ActualCurrency = actualCurrency;       
    }
    
    public Currency ExpectedCurrency { get; }
    
    public Currency ActualCurrency { get; }
}