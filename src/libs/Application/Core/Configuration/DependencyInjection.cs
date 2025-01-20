using Application.Core.Behaviors;
using Application.Core.OperatingRules;
using Application.Core.Processors;
using Application.Prospecting.Addresses.Queries.SearchAddressByZipCode;
using Application.Prospecting.Leads.Commands.BulkInsertLead;
using Application.Prospecting.Leads.Commands.RegisterLead;
using Application.Prospecting.Leads.Commands.RemoveLead;
using Application.Prospecting.Leads.Commands.UpdateLead;
using Application.Prospecting.Leads.Queries.GetLeadById;
using Application.Prospecting.Leads.Queries.SearchLead;
using Application.Security.OneTimePassword.Commands.GenerateOneTimePassword;
using Application.Security.OneTimePassword.Commands.HandleOneTimePassword;
using CrossCutting.Caching.Configuration;
using CrossCutting.Csv.Configuration;
using CrossCutting.EndUserCommunication;
using CrossCutting.FileStorage.Configuration;
using CrossCutting.Logging.Configuration;
using CrossCutting.Messaging.Configuration;
using CrossCutting.Monitoring.Configuration;
using CrossCutting.Security;
using Domain.Core;
using Domain.Prospecting.Entities;
using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.ApplicationOperationRules;
using Shared.Results;
using ViaCep.ServiceClient.Configuration;

namespace Application.Core.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment hostingEnvironment)
    {
        var applicationAssemblyRef = typeof(DependencyInjection).Assembly;
        var coreAssemblyRef = typeof(IEntity).Assembly;

        services.AddValidatorsFromAssembly(applicationAssemblyRef)
                .AddMediatR(config =>
                {
                    config.RegisterServicesFromAssemblies(coreAssemblyRef, applicationAssemblyRef)
                          .RegisterProcessors(services)
                          .RegisterBehaviors(services);
                })
                .AddIntegrationClientServices(configuration)
                .AddCrossCuttingServices(configuration)
                .AddEndUserCommunicationServices(configuration)
                .AddAplicationOperatingRules(configuration);

		return services;
    }

    private static IServiceCollection AddIntegrationClientServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddViaCepIntegrationServiceClient(configuration);

        return services;
    }

	private static IServiceCollection AddAplicationOperatingRules(this IServiceCollection services, IConfiguration configuration)
		=> services.AddSingleton<IApplicationOperatingRule, BusinessHoursOnlyOperatingRule>()
				   .AddSingleton<IApplicationOperatingRule, WeekDaysOnlyOperatingRule>();

	private static IServiceCollection AddCrossCuttingServices(this IServiceCollection services, IConfiguration configuration)
        => services.AddCsvHelper(configuration)
                   .AddFileStorage(configuration)
                   .AddMultiSinkLogging(configuration)
                   .AddCaching(configuration)
                   .AddMessageBus(configuration)
                   .AddSecurity(configuration)
                   .AddLeadManagerApiMonitoring(configuration);

    private static MediatRServiceConfiguration RegisterBehaviors(this MediatRServiceConfiguration config, IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));

        return config.RegisterValidationBehaviors();
    }

    private static MediatRServiceConfiguration RegisterProcessors(this MediatRServiceConfiguration config, IServiceCollection services)
    {
        config.AddOpenRequestPreProcessor(typeof(ApplicationOperatingRulesProcessor<>)); //O que me ajudou a fazer este PreProcessor funcionar: https://github.com/jbogard/MediatR/issues/868
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
                 .AddBehavior<IPipelineBehavior<BulkInsertLeadCommandRequest, ApplicationResponse<BulkInsertLeadCommandResponse>>, ValidationBehavior<BulkInsertLeadCommandRequest, BulkInsertLeadCommandResponse>>()
                 .AddBehavior<IPipelineBehavior<GenerateOneTimePasswordCommandRequest, ApplicationResponse<GenerateOneTimePasswordCommandResponse>>, ValidationBehavior<GenerateOneTimePasswordCommandRequest, GenerateOneTimePasswordCommandResponse>>()
                 .AddBehavior<IPipelineBehavior<HandleOneTimePasswordCommandRequest, ApplicationResponse<HandleOneTimePasswordCommandResponse>>, ValidationBehavior<HandleOneTimePasswordCommandRequest, HandleOneTimePasswordCommandResponse>>();
}