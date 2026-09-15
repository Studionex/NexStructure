using MediatR;

namespace NexStructure.Domain.Common.Models;

public abstract record DomainEvent(Guid Id = new()) : INotification;
