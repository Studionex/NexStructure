namespace NexStructure.Domain.Common.Models;

public interface IEntity
{
    IEnumerable<DomainEvent> DomainEvents { get; }
    void ClearDomainEvents();

}
