using Application.Behaviors;
using Application.Features.Addresses.Queries.SearchAddressByZipCode;
using Application.Features.Leads.Commands.BulkInsertLead;
using Application.Features.Leads.Commands.RegisterLead;
using Application.Features.Leads.Commands.RemoveLead;
using Application.Features.Leads.Commands.UpdateLead;
using Application.Features.Leads.Queries.GetLeadById;
using Application.Features.Leads.Queries.SearchLead;
using Application.Features.Leads.Shared;
using Application.Processors;
using Core.Entities;
using CrossCutting.Caching.Configuration;
using CrossCutting.Csv.Configuration;
using CrossCutting.FileStorage.Configuration;
using CrossCutting.Logging.Configuration;
using CrossCutting.Messaging.Configuration;
using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Results;
using ViaCep.ServiceClient.Configuration;

namespace Application.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var applicationAssemblyRef = typeof(DependencyInjection).Assembly;
        var coreAssemblyRef = typeof(IEntity).Assembly;

        services.AddValidatorsFromAssembly(applicationAssemblyRef)
                .AddMediatR(config =>
                {
                    config.RegisterServicesFromAssemblies(coreAssemblyRef, applicationAssemblyRef)
                          .RegisterServicesFromAssembly(applicationAssemblyRef)
                          .RegisterBehaviors(services)
                          .RegisterProcessors(services);
                })
                .AddIntegrationClientServices(configuration)
                .AddCrossCuttingServices(configuration);

        return services;
    }

    private static IServiceCollection AddIntegrationClientServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddViaCepIntegrationServiceClient(configuration);

        return services;
    }

    private static IServiceCollection AddCrossCuttingServices(this IServiceCollection services, IConfiguration configuration)
        => services.AddCsvHelper(configuration)
                   .AddFileStorage(configuration)
                   .AddMultiSinkLogging(configuration)
                   .AddCaching(configuration)
                   .AddMessageBus(configuration);

    private static MediatRServiceConfiguration RegisterBehaviors(this MediatRServiceConfiguration config, IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));

        return config.RegisterValidationBehaviors();
    }

    private static MediatRServiceConfiguration RegisterProcessors(this MediatRServiceConfiguration config, IServiceCollection services)
    {
        services.AddTransient(typeof(IRequestPostProcessor<,>), typeof(EventHandlerDispatchingProcessor<,>));

        return config;
    }

    private static MediatRServiceConfiguration RegisterValidationBehaviors(this MediatRServiceConfiguration config)
        => config.AddBehavior<IPipelineBehavior<GetLeadByIdQueryRequest, ApplicationResponse<LeadDto>>, ValidationBehavior<GetLeadByIdQueryRequest, LeadDto>>()
                 .AddBehavior<IPipelineBehavior<SearchAddressByZipCodeQueryRequest, ApplicationResponse<SearchAddressByZipCodeQueryResponse>>, ValidationBehavior<SearchAddressByZipCodeQueryRequest, SearchAddressByZipCodeQueryResponse>>()
                 .AddBehavior<IPipelineBehavior<RegisterLeadCommandRequest, ApplicationResponse<RegisterLeadCommandResponse>>, ValidationBehavior<RegisterLeadCommandRequest, RegisterLeadCommandResponse>>()
                 .AddBehavior<IPipelineBehavior<UpdateLeadCommandRequest, ApplicationResponse<UpdateLeadCommandResponse>>, ValidationBehavior<UpdateLeadCommandRequest, UpdateLeadCommandResponse>>()
                 .AddBehavior<IPipelineBehavior<RemoveLeadCommandRequest, ApplicationResponse<RemoveLeadCommandResponse>>, ValidationBehavior<RemoveLeadCommandRequest, RemoveLeadCommandResponse>>()
                 .AddBehavior<IPipelineBehavior<SearchLeadQueryRequest, ApplicationResponse<bool>>, ValidationBehavior<SearchLeadQueryRequest, bool>>()
                 .AddBehavior<IPipelineBehavior<BulkInsertLeadCommandRequest, ApplicationResponse<BulkInsertLeadCommandResponse>>, ValidationBehavior<BulkInsertLeadCommandRequest, BulkInsertLeadCommandResponse>>();
}