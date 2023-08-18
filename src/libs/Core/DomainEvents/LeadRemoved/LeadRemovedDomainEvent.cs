using Core.Entities;
using Shared.Events.DomainEvents;

namespace Core.DomainEvents.LeadRemoved;

public sealed record LeadRemovedDomainEvent(Lead Lead) : IDomainEvent;