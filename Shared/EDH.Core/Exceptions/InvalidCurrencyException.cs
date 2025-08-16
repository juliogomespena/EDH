using EDH.Core.Enums;

namespace EDH.Core.Exceptions;

public sealed class InvalidCurrencyException : DomainValidationException
{
    public InvalidCurrencyException(Currency expectedCurrency, Currency actualCurrency) : base($"Actual currency \"{actualCurrency.ToString()}\" is not valid. Expected \"{expectedCurrency.ToString()}\".")
    {       
        ExpectedCurrency = expectedCurrency;
        ActualCurrency = actualCurrency;       
    }
    
    public InvalidCurrencyException(params Currency[] currencies) : base($"There is a mismatch between the currencies: {GetAllCurrenciesFromParams(currencies)}.")
    {       
    }
    
    public Currency ExpectedCurrency { get; }
    
    public Currency ActualCurrency { get; }

    private static string GetAllCurrenciesFromParams(Currency[] currencies) => String.Join(", ", currencies.Distinct());
}