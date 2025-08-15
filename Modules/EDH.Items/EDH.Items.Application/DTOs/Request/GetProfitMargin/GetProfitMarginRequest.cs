using EDH.Core.Enums;

namespace EDH.Items.Application.DTOs.Request.GetProfitMargin;

public sealed record GetProfitMarginRequest(decimal Price, decimal Costs, Currency Currency);