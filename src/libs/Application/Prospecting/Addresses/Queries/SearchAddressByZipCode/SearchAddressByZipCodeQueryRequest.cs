using MediatR;
using Shared.Results;

namespace Application.Prospecting.Addresses.Queries.SearchAddressByZipCode;

//public sealed class SearchAddressByZipCodeQueryRequest : IRequest<ApplicationResponse<SearchAddressByZipCodeQueryResponse>>
//{
//    public required string Cep { get; init; }
//}

public sealed record SearchAddressByZipCodeQueryRequest(string ZipCode) : IRequest<ApplicationResponse<SearchAddressByZipCodeQueryResponse>>;