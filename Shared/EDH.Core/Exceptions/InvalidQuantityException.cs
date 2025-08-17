namespace EDH.Core.Exceptions;

public sealed class InvalidQuantityException(string? message = null) :
    DomainValidationException(message ?? "Quantity should be equal or greater than zero.");