using Domain.Prospecting.Entities;
using Shared.Events.DomainEvents;

namespace Domain.Prospecting.DomainEvents.LeadRemoved;

public sealed record LeadRemovedDomainEvent(Lead Lead) : IDomainEvent;