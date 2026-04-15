namespace FixNet.Domain.Base;

public abstract class AggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyList<IDomainEvent> Events => _domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void MarkChangesAsCommitted()
    {
        _domainEvents.Clear();
    }

    public void AddDomainEvents(IEnumerable<IDomainEvent> events)
    {
        foreach (var @event in events)
        {
            RaiseDomainEvent(@event);
        }
    }
}