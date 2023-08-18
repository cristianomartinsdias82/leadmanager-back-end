using MediatR;
using Shared.Results;

namespace Application.Features.Leads.Commands.BulkInsertLead;

public sealed class BulkInsertLeadCommandRequest : IRequest<ApplicationResponse<BulkInsertLeadCommandResponse>>
{
    public Stream InputStream { get; init; } = default!;
    public string FileName { get; init; } = default!;
    public string ContentType { get; init; } = default!;
    public string FileUpload_MaxSizeInBytesConfigurationKeyName { get; init; } = default!;
    public long ContentSizeInBytes { get; init; } = default!;
}