using Application.Core.Contracts.Persistence;
using Application.Prospecting.Leads.Shared;

namespace Application.Prospecting.Leads.Commands.RemoveLead;

public sealed record RemoveLeadCommandResponse(
    RecordStates? RecordState,
    RevisionUpdate? RevisionUpdate,
    LeadDto? LeadData);
