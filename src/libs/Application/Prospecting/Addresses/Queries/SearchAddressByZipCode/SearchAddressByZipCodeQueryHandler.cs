using Application.AddressSearch.Contracts;
using MediatR;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Prospecting.Addresses.Queries.SearchAddressByZipCode;

internal sealed class SearchAddressByZipCodeQueryHandler : ApplicationRequestHandler<SearchAddressByZipCodeQueryRequest, SearchAddressByZipCodeQueryResponse>
{
	private const string AddressNotFound = "Endereço não localizado.";

	private readonly IAddressSearch _addressSearch;

	public SearchAddressByZipCodeQueryHandler(
		IMediator mediator,
		IAddressSearch addressSearch) : base(mediator, default!)
	{
		_addressSearch = addressSearch;
	}

	public override async Task<ApplicationResponse<SearchAddressByZipCodeQueryResponse>> Handle(SearchAddressByZipCodeQueryRequest request, CancellationToken cancellationToken)
	{
		var searchAddressResult = await _addressSearch.SearchByZipCodeAsync(request.ZipCode, cancellationToken);
		if (searchAddressResult == default)
			return ApplicationResponse<SearchAddressByZipCodeQueryResponse>
					.Create(null!, message: AddressNotFound, operationCode: OperationCodes.NotFound);

		return
			ApplicationResponse<SearchAddressByZipCodeQueryResponse>
				.Create(SearchAddressByZipCodeQueryResponse.FromModel(searchAddressResult));
	}
}