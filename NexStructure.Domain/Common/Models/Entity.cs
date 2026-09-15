using System.ComponentModel.DataAnnotations.Schema;
namespace NexStructure.Domain.Common.Models;

public abstract record Entity<TId> : IEntity
{
  public TId Id { get; init; }

  protected Entity()
  {

  }

  protected Entity(TId id)
  {


    Id = id;
  }

  private readonly List<DomainEvent> _domainEvents = [];

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  [NotMapped] public IEnumerable<DomainEvent> DomainEvents => _domainEvents;

  public void ClearDomainEvents()
  {
    _domainEvents.Clear();
  }

  protected void RiseDomainEvent(DomainEvent domainEvent)
  {
    _domainEvents.Add(domainEvent);
  }


}
