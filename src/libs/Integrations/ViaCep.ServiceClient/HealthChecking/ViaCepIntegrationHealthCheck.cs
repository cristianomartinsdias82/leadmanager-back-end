using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ViaCep.ServiceClient.HealthChecking;

internal sealed class ViaCepIntegrationHealthCheck : IHealthCheck
{
    private readonly IViaCepServiceClient _serviceClient;

    public ViaCepIntegrationHealthCheck(IViaCepServiceClient serviceClient)
    {
        _serviceClient = serviceClient;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _serviceClient.SearchAsync("04858040", cancellationToken);

            return HealthCheckResult.Healthy();
        }
        catch(Exception exc)
        {
            return HealthCheckResult.Degraded(exception: exc);
        }
    }
}
