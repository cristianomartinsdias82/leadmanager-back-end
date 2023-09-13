using Application.Contracts.Persistence;
using Application.Features.Leads.Shared;
using Core.Entities;

namespace Application.Features.Leads.Commands.RemoveLead;

public sealed record RemoveLeadCommandResponse(
    RecordStates? RecordState,
    RevisionUpdate? RevisionUpdate,
    LeadDto? LeadData);
