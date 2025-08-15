namespace EDH.Core.Exceptions;

public sealed class InvalidDiscountException() : 
    ArgumentException("Discount value should be zero or negative number.");

public sealed class InvalidSurchargeException()
    : ArgumentException("Surcharge value should be zero or positive number.");