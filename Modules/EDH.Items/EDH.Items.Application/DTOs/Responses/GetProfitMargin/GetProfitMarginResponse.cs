using EDH.Core.Enums;

namespace EDH.Items.Application.DTOs.Responses.GetProfitMargin;

public sealed record GetProfitMarginResponse(decimal Value, decimal Percentage, Currency Currency);