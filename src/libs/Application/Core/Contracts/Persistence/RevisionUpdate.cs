namespace Application.Core.Contracts.Persistence;

public record struct RevisionUpdate(Guid Id, byte[] Revision);
