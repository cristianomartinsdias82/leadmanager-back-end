using LeadManagerApi.Configuration;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace LeadManagerApi.ApiFeatures;

public class ApiKeyHeaderOperationFilter : IOperationFilter
{
    private readonly LeadManagerApiSettings _apiSettings;

    public ApiKeyHeaderOperationFilter(LeadManagerApiSettings apiSettings)
    {
        _apiSettings = apiSettings;
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = _apiSettings.ApiKeyRequestHeaderName,
            In = ParameterLocation.Header,
            Description = "The Lead Manager Api Key",
            Required = true
        });
    }
}