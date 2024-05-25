using FluentAssertions;
using NetArchTest.Rules;
using System.Reflection;
using AppLayer = Application;
using DomainLayer = Domain;
using InfraLayer = Infrastructure;
using PresentationLayer = LeadManagerApi;

namespace ArchitectureValidation.Tests;

//For more info, read the following:
//https://github.com/BenMorris/NetArchTest?tab=readme-ov-file#readme
//https://www.milanjovanovic.tech/blog/shift-left-with-architecture-testing-in-dotnet?utm_source=newsletter&utm_medium=email&utm_campaign=tnw91
//https://code-maze.com/csharp-architecture-tests-with-netarchtest-rules/

public class AssemblyDependenciesTests
{
    private static readonly Assembly DomainAssembly = Assembly.GetAssembly(typeof(DomainLayer.Core.IEntity))!;
    private static readonly Assembly ApplicationAssembly = Assembly.GetAssembly(typeof(AppLayer.Core.Configuration.DependencyInjection))!;
    private static readonly Assembly InfrastructureAssembly = Assembly.GetAssembly(typeof(InfraLayer.Configuration.DependencyInjection))!;
    private static readonly Assembly PresentationAssembly = Assembly.GetAssembly(typeof(PresentationLayer.Program))!;
    private static readonly Assembly SharedAssembly = Assembly.GetAssembly(typeof(Shared.DataPagination.PaginationOptions))!;

    [Fact]
    public void DomainLayer_ShouldNotHaveDependencyOn_ApplicationLayer()
    {
        Types
            .InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn(ApplicationAssembly.GetName().Name)
            .GetResult().IsSuccessful
            .Should().BeTrue();
    }

    [Fact]
    public void ApplicationLayer_ShouldNotHaveDependencyOn_InfrastructureLayer()
    {
        Types
            .InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOn(InfrastructureAssembly.GetName().Name)
            .GetResult().IsSuccessful
            .Should().BeTrue();
    }

    [Fact]
    public void SharedLayer_ShouldNotHaveDependencyOn_AnyLayer()
    {
        Types
            .InAssembly(SharedAssembly)
            .Should()
            .NotHaveDependencyOnAll(
                PresentationAssembly.GetName().Name,
                ApplicationAssembly.GetName().Name,
                DomainAssembly.GetName().Name,
                InfrastructureAssembly.GetName().Name)
            .GetResult().IsSuccessful
            .Should().BeTrue();
    }
}