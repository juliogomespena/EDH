namespace EDH.Core.Events.Abstractions;

public sealed class SubscriptionToken : IDisposable
{
    private readonly Action? _unsubscribeAction;
    private bool _disposed;
    
    public SubscriptionToken(Action? unsubscribeAction = null)
    {
        _unsubscribeAction = unsubscribeAction;
    }
    
    public void Dispose()
    {
        if (_disposed) return;
        
        _unsubscribeAction?.Invoke();
        _disposed = true;
    }
}