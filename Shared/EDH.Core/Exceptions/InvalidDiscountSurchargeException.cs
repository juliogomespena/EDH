namespace EDH.Core.Exceptions;

public abstract class InvalidDiscountSurchargeException(string message) : DomainValidationException(message);

public sealed class InvalidDiscountException() : 
    InvalidDiscountSurchargeException("Discount value should be zero or negative number.");

public sealed class InvalidSurchargeException()
    : InvalidDiscountSurchargeException("Surcharge value should be zero or positive number.");