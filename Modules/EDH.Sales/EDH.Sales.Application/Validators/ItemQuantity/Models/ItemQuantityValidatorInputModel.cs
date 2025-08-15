namespace EDH.Sales.Application.Validators.ItemQuantity.Models;

public sealed record ItemQuantityValidatorInputModel (string? ItemQuantity, bool HasSelectedItem, int? AvailableStock);