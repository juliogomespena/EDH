using System.Net.Sockets;
using System.Runtime.InteropServices;
using EDH.Core.Enums;

namespace EDH.Core.Helpers;

public static class ExceptionPolicyHelper
{
    public static ExceptionCategory Categorize(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);
        
        if (exception is AggregateException { InnerException: { } inner })
            exception = inner;
        
        return exception switch
        {
            not null when IsBenign(exception) => ExceptionCategory.Benign,
            not null when IsTypicalUiUsageError(exception) => ExceptionCategory.Recoverable,
            not null when IsLikelyTransientIo(exception) => ExceptionCategory.Recoverable,
            not null when IsClipboardComNoise(exception) => ExceptionCategory.Recoverable,
            not null when IsDefinitelyFatal(exception) => ExceptionCategory.Fatal,
            _ => ExceptionCategory.Fatal,
        };
    }
    
    private static bool IsBenign(Exception ex) =>
        ex is OperationCanceledException or TaskCanceledException;
    
    private static bool IsTypicalUiUsageError(Exception ex) =>
        ex is ArgumentException
        or ArgumentNullException
        or ArgumentOutOfRangeException
        or InvalidOperationException
        or FormatException
        or NotSupportedException
        or InvalidDataException;
    
    private static bool IsLikelyTransientIo(Exception ex) =>
        ex is IOException
        or HttpRequestException
        or TimeoutException
        or SocketException;
    
    private static bool IsClipboardComNoise(Exception ex) =>
        ex is COMException { HResult: unchecked((int)0x800401D0) or unchecked((int)0x800401D3) };
    
    private static bool IsDefinitelyFatal(Exception ex) =>
        ex is OutOfMemoryException or 
            StackOverflowException or 
            AccessViolationException or 
            SEHException or 
            AppDomainUnloadedException or 
            BadImageFormatException or 
            TypeInitializationException or 
            ThreadAbortException or 
            InsufficientExecutionStackException;
}