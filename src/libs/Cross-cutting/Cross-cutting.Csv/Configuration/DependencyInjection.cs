using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CrossCutting.Csv.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddCsvHelper(this IServiceCollection services)
    {
        services.TryAddSingleton<ICsvHelper, CsvHelper>();

        return services;
    }
}