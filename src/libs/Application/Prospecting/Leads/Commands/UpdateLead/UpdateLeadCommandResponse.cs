using Application.Core.Contracts.Persistence;
using Application.Prospecting.Leads.Shared;

namespace Application.Prospecting.Leads.Commands.UpdateLead;

public sealed record UpdateLeadCommandResponse(
    RecordStates? RecordState,
    RevisionUpdate? RevisionUpdate,
    LeadDto? LeadData);
