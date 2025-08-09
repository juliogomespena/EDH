using EDH.Core.Enums;
using EDH.Core.Helpers;
using EDH.Infrastructure.Common.Exceptions.Enums;
using EDH.Infrastructure.Common.Exceptions.Interface;
using Microsoft.Extensions.Logging;

namespace EDH.Infrastructure.Common.Exceptions;

public sealed class GlobalExceptionCoordinator : IGlobalExceptionCoordinator
{
    private readonly ILogger<GlobalExceptionCoordinator> _logger;

    public GlobalExceptionCoordinator(ILogger<GlobalExceptionCoordinator> logger)
    {
        _logger = logger;
    }

    public HandlingResult Handle(Exception exception, bool onUiThread, string context = "")
    {
        var category = ExceptionPolicyHelper.Categorize(exception);

        switch (category)
        {
            case ExceptionCategory.Benign:
            {
                _logger.LogWarning(exception, "Benign exception occurred ({Context})", context);
                return HandlingResult.Continue;
            }

            case ExceptionCategory.Recoverable:
            {
                _logger.LogError(exception, "Recoverable exception occurred ({Context})", context);
                return onUiThread ? HandlingResult.Continue : HandlingResult.Exit;
            }

            case ExceptionCategory.Fatal:
            default:
            {
                _logger.LogCritical(exception, "Fatal exception occurred ({Context})", context);
                return HandlingResult.Exit;
            }
        }
    }
}