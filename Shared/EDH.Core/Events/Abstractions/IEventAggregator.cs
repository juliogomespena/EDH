namespace EDH.Core.Events.Abstractions;

public interface IEventAggregator
{
    void Publish<TEvent>() where TEvent : class, new();
    
    void Publish<TEvent, TPayload>(TPayload payload) 
        where TEvent : class, new();
    
    SubscriptionToken Subscribe<TEvent>(Action action) 
        where TEvent : class, new();
    
    SubscriptionToken Subscribe<TEvent, TPayload>(Action<TPayload> action) 
        where TEvent : class, new();
    
    SubscriptionToken Subscribe<TEvent, TPayload>(
        Action<TPayload> action, 
        Predicate<TPayload> filter) 
        where TEvent : class, new();
}