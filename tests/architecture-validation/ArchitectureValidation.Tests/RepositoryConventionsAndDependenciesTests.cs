using Application.Core.Contracts.Repository;
using FluentAssertions;
using Infrastructure.Repository;
using NetArchTest.Rules;
using System.Reflection;
using InfraLayer = Infrastructure;

namespace ArchitectureValidation.Tests;

public class RepositoryConventionsAndDependenciesTests
{
    private static readonly Assembly InfrastructureAssembly = Assembly.GetAssembly(typeof(InfraLayer.Configuration.DependencyInjection))!;

    //This is not where this method belongs
    [Fact]
    public void InfrastructureLayer_ConcreteRepositoryClasses_ShouldBeNotPublic_And_ShouldBeSealed()
    {
        Types
            .InAssembly(InfrastructureAssembly)
            .That()
            .Inherit(typeof(RepositoryBase<>))
            .Should()
            .NotBePublic()
            .And()
            .BeSealed()
            .GetResult().IsSuccessful
            .Should().BeTrue();
    }

    //This is not where this method belongs
    [Fact]
    public void InfrastructureLayer_RepositoryClasses_ShouldResideInRepositoryNamespace()
    {
        Types
            .InAssembly(InfrastructureAssembly)
            .That()
            .Inherit(typeof(RepositoryBase<>))
            .Or()
            .ImplementInterface(typeof(IRepository<>))
            .Should()
            .ResideInNamespace("Infrastructure.Repository")
            .GetResult().IsSuccessful
            .Should().BeTrue();
    }

    //This is not where this method belongs
    [Fact]
    public void InfrastructureLayer_RepositoryClasses_ShouldHaveNameEndingWithRepository()
    {
        Types
            .InAssembly(InfrastructureAssembly)
            .That()
            .Inherit(typeof(IRepository<>))//.ImplementInterface(typeof(IRepository<>))
            .Or()
            .Inherit(typeof(RepositoryBase<>))
            .Should()
            .HaveNameEndingWith("Repository")
            .GetResult().IsSuccessful
            .Should().BeTrue();
    }
}
