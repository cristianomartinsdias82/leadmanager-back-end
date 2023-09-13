using Application.Contracts.Persistence;
using Application.Features.Leads.Shared;
using Core.Entities;

namespace Application.Features.Leads.Commands.UpdateLead;

public sealed record UpdateLeadCommandResponse(
    RecordStates? RecordState,
    RevisionUpdate? RevisionUpdate,
    LeadDto? LeadData);
