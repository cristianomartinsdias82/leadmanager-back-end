using Application.AddressSearch.Models;

namespace Application.AddressSearch.Contracts;

public interface IAddressSearch
{
	Task<Address?> SearchByZipCodeAsync(
					string zipCode,
					CancellationToken cancellationToken = default);
}
