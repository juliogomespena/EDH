using EDH.Infrastructure.Common.Exceptions.Enums;

namespace EDH.Infrastructure.Common.Exceptions.Interface;

public interface IGlobalExceptionCoordinator
{
    HandlingResult Handle(Exception exception, bool onUiThread, string context = "");
}