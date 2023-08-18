using Core.Entities;
using Shared.Events.DomainEvents;

namespace Core.DomainEvents.LeadUpdated;

public sealed record LeadUpdatedDomainEvent(Lead Lead) : IDomainEvent;