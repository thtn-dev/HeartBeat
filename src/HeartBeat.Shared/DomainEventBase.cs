using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using MediatR;

namespace HeartBeat.Shared;

public abstract record DomainEventBase : INotification
{
    public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
}

public class HasDomainEventBase
{
    private readonly List<DomainEventBase> _domainEvents = [];
    
    [NotMapped]
    [JsonIgnore]
    public IReadOnlyCollection<DomainEventBase> DomainEvents => _domainEvents.AsReadOnly();

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected void RegisterDomainEvent(DomainEventBase domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}

public interface IDomainEventDispatcher
{
    Task DispatchAndClearEvents<TId>(IEnumerable<EntityAggregateBase<TId>> entitiesWithEvents) where TId : IEquatable<TId>;

    Task DispatchAndClearEvents(IEnumerable<HasDomainEventBase> domainEvents);
}

public class MediatrDomainEventDispatcher(IMediator mediator) : IDomainEventDispatcher
{
    public async Task DispatchAndClearEvents<TId>(IEnumerable<EntityAggregateBase<TId>> entitiesWithEvents) where TId : IEquatable<TId>
    {
        foreach (var entity in entitiesWithEvents)
        {
            var @events = entity.DomainEvents.ToArray();
            entity.ClearDomainEvents();
            foreach (var @event in @events)
            {
                await mediator.Publish(@event).ConfigureAwait(false);
            }
        }
    }

    public async Task DispatchAndClearEvents(IEnumerable<HasDomainEventBase> domainEvents)
    {
        foreach (var entity in domainEvents)
        {
            var @events = entity.DomainEvents.ToArray();
            entity.ClearDomainEvents();
            foreach (var @event in @events)
            {
                await mediator.Publish(@event).ConfigureAwait(false);
            }
        }
    }
}