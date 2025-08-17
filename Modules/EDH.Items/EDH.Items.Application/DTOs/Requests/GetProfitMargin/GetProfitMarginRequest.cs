using EDH.Core.Enums;

namespace EDH.Items.Application.DTOs.Requests.GetProfitMargin;

public sealed record GetProfitMarginRequest(decimal Price, decimal Costs, Currency Currency);