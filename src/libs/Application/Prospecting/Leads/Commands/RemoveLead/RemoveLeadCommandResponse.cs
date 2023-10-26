using Application.Core.Contracts.Persistence;
using Domain.Prospecting.Entities;

namespace Application.Prospecting.Leads.Commands.RemoveLead;

public sealed record RemoveLeadCommandResponse(
    RecordStates? RecordState,
    RevisionUpdate? RevisionUpdate,
    LeadDto? LeadData);
