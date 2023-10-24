using Domain.Prospecting.Entities;
using Shared.Events.DomainEvents;

namespace Domain.Prospecting.DomainEvents.LeadBulkInserted;

public sealed record LeadBulkInsertedDomainEvent(List<Lead> Leads) : IDomainEvent;