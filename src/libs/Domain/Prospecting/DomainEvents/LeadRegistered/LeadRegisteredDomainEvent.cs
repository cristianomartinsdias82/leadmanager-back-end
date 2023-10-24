using Domain.Prospecting.Entities;
using Shared.Events.DomainEvents;

namespace Domain.Prospecting.DomainEvents.LeadRegistered;

public sealed record LeadRegisteredDomainEvent(Lead Lead) : IDomainEvent;