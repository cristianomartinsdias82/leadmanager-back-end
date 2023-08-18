using Core.Entities;
using Shared.Events.DomainEvents;

namespace Core.DomainEvents.LeadRegistered;

public sealed record LeadRegisteredDomainEvent(Lead Lead) : IDomainEvent;