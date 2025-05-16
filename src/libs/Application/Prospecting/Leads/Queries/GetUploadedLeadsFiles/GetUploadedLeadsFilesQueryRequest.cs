using MediatR;
using Shared.DataQuerying;
using Shared.Results;

namespace Application.Prospecting.Leads.Queries.GetUploadedLeadsFiles;

public sealed record GetUploadedLeadsFilesQueryRequest(PaginationOptions PaginationOptions)
						: IRequest<ApplicationResponse<PagedList<UploadedLeadsFileDto>>>;