using SubscriptionToken = EDH.Core.Events.Abstractions.SubscriptionToken;

namespace EDH.Infrastructure.Common.Events;

public sealed class PrismEventAggregatorAdapter : Core.Events.Abstractions.IEventAggregator
{
    private readonly Prism.Events.IEventAggregator _prismEventAggregator;
    
    public PrismEventAggregatorAdapter(Prism.Events.IEventAggregator prismEventAggregator)
    {
        _prismEventAggregator = prismEventAggregator ?? 
            throw new ArgumentNullException(nameof(prismEventAggregator));
    }
    
    public void Publish<TEvent>() where TEvent : class, new()
    {
        _prismEventAggregator
            .GetEvent<PrismEventProxy<TEvent>>()
            .Publish();
    }
    
    public void Publish<TEvent, TPayload>(TPayload payload) where TEvent : class, new()
    {
        _prismEventAggregator
            .GetEvent<PrismEventProxyWithPayload<TEvent, TPayload>>()
            .Publish(payload);
    }
    
    public SubscriptionToken Subscribe<TEvent>(Action action) where TEvent : class, new()
    {
        var prismEvent = _prismEventAggregator.GetEvent<PrismEventProxy<TEvent>>();
        var prismToken = prismEvent.Subscribe(action);
        
        return new SubscriptionToken(() => prismEvent.Unsubscribe(prismToken));
    }
    
    public SubscriptionToken Subscribe<TEvent, TPayload>(Action<TPayload> action) 
        where TEvent : class, new()
    {
        var prismEvent = _prismEventAggregator
            .GetEvent<PrismEventProxyWithPayload<TEvent, TPayload>>();
        var prismToken = prismEvent.Subscribe(action);
        
        return new SubscriptionToken(() => prismEvent.Unsubscribe(prismToken));
    }
    
    public SubscriptionToken Subscribe<TEvent, TPayload>(
        Action<TPayload> action, 
        Predicate<TPayload> filter) 
        where TEvent : class, new()
    {
        var prismEvent = _prismEventAggregator
            .GetEvent<PrismEventProxyWithPayload<TEvent, TPayload>>();
        var prismToken = prismEvent.Subscribe(action, ThreadOption.PublisherThread, false, filter);
        
        return new SubscriptionToken(() => prismEvent.Unsubscribe(prismToken));
    }
    
    public void Unsubscribe(SubscriptionToken token)
    {
        token?.Dispose();
    }
    
    private class PrismEventProxy<TEvent> : PubSubEvent 
        where TEvent : class, new()
    {
    }
    
    private class PrismEventProxyWithPayload<TEvent, TPayload> : PubSubEvent<TPayload> 
        where TEvent : class, new()
    {
    }
}