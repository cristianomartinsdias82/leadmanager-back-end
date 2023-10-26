using Application.Core.Contracts.Persistence;
using Domain.Prospecting.Entities;

namespace Application.Prospecting.Leads.Commands.UpdateLead;

public sealed record UpdateLeadCommandResponse(
    RecordStates? RecordState,
    RevisionUpdate? RevisionUpdate,
    LeadDto? LeadData);
