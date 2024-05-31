using FluentAssertions;
using NetArchTest.Rules;
using Shared.RequestHandling;
using System.Reflection;
using AppLayer = Application;

namespace ArchitectureValidation.Tests;

public class HandlerConventionsAndDependenciesTests
{
    private static readonly Assembly ApplicationAssembly = Assembly.GetAssembly(typeof(AppLayer.Core.Configuration.DependencyInjection))!;

    [Fact]
    public void Handlers_ShouldHave_NameEndingWith_Handler()
    {
        Types.InAssembly(ApplicationAssembly)
            .That()
            .Inherit(typeof(ApplicationRequestHandler<,>))
            .Should()
            .HaveNameEndingWith("Handler")
            .GetResult().IsSuccessful
            .Should().BeTrue();
    }

    [Fact]
    public void Handlers_ShouldBeNotPublic_And_ShouldBeSealed()
    {
        Types.InAssembly(ApplicationAssembly)
            .That()
            .Inherit(typeof(ApplicationRequestHandler<,>))
            .Should()
            .NotBePublic()
            .And()
            .BeSealed()
            .GetResult().IsSuccessful
            .Should().BeTrue();
    }
}
