using Core.Entities;
using Shared.Events.DomainEvents;

namespace Core.DomainEvents.LeadBulkInserted;

public sealed record LeadBulkInsertedDomainEvent(List<Lead> Leads) : IDomainEvent;