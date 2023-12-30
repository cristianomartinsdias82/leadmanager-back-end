using Domain.Prospecting.Entities;
using Shared.Events.DomainEvents;

namespace Domain.Prospecting.DomainEvents.LeadUpdated;

public sealed record LeadUpdatedDomainEvent(Lead Lead) : IDomainEvent;