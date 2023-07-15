using Application.Behaviors;
using Application.Features.Addresses.Queries.SearchAddressByZipCode;
using Application.Features.Leads.Commands.RegisterLead;
using Application.Features.Leads.Commands.RemoveLead;
using Application.Features.Leads.Commands.UpdateLead;
using Application.Features.Leads.Queries.GetLeadById;
using Application.Features.Leads.Queries.SearchLead;
using Application.Features.Leads.Queries.Shared;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Results;
using ViaCep.ServiceClient.Configuration;

namespace Application.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var assemblyRef = typeof(DependencyInjection).Assembly;

        services.AddValidatorsFromAssembly(assemblyRef);
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assemblyRef);
            config.RegisterValidationBehaviors();
        });

        services.AddIntegrationClientServices(configuration);

        return services;
    }

    public static IServiceCollection AddIntegrationClientServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddViaCepIntegrationServiceClient(configuration);

        return services;
    }

    private static MediatRServiceConfiguration RegisterValidationBehaviors(this MediatRServiceConfiguration config)
    {
        config.AddBehavior<IPipelineBehavior<GetLeadByIdQueryRequest, ApplicationResponse<LeadDto>>,
                            ValidationBehavior<GetLeadByIdQueryRequest, LeadDto>>();

        config.AddBehavior<IPipelineBehavior<SearchAddressByZipCodeQueryRequest, ApplicationResponse<SearchAddressByZipCodeQueryResponse>>,
                            ValidationBehavior<SearchAddressByZipCodeQueryRequest, SearchAddressByZipCodeQueryResponse>>();

        config.AddBehavior<IPipelineBehavior<RegisterLeadCommandRequest, ApplicationResponse<RegisterLeadCommandResponse>>,
                            ValidationBehavior<RegisterLeadCommandRequest, RegisterLeadCommandResponse>>();

        config.AddBehavior<IPipelineBehavior<UpdateLeadCommandRequest, ApplicationResponse<UpdateLeadCommandResponse>>,
                            ValidationBehavior<UpdateLeadCommandRequest, UpdateLeadCommandResponse>>();

        config.AddBehavior<IPipelineBehavior<RemoveLeadCommandRequest, ApplicationResponse<RemoveLeadCommandResponse>>,
                            ValidationBehavior<RemoveLeadCommandRequest, RemoveLeadCommandResponse>>();

        config.AddBehavior<IPipelineBehavior<SearchLeadQueryRequest, ApplicationResponse<bool>>,
                            ValidationBehavior<SearchLeadQueryRequest, bool>>();

        return config;
    }
}