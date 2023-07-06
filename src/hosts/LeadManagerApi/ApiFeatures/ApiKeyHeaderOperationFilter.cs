using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace LeadManagerApi.ApiFeatures;

public class ApiKeyHeaderOperationFilter : IOperationFilter
{
    private readonly LeadManagerApiSettings _apiSettings;

    public ApiKeyHeaderOperationFilter(IOptions<LeadManagerApiSettings> apiSettings)
    {
        _apiSettings = apiSettings.Value;
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = _apiSettings.ApiKeyRequestHeaderName,
            In = ParameterLocation.Header,
            Description = "The Lead Manager Api Key",
            Required = false,
            Schema = new OpenApiSchema { Type = "String" }
        });
    }
}