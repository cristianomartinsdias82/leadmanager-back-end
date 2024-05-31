using Domain.Core;
using FluentAssertions;
using NetArchTest.Rules;
using System.Reflection;
using DomainLayer = Domain;

namespace ArchitectureValidation.Tests;

public class EntityDesignRulesTests
{
    private static readonly Assembly DomainAssembly = Assembly.GetAssembly(typeof(DomainLayer.Core.IEntity))!;

    [Fact]
    public void Entities_ShouldOnlyHave_PrivateConstructors()
    {
        IEnumerable<Type> entityTypes = Types.InAssembly(DomainAssembly)
            .That()
            .Inherit(typeof(Entity))
            .GetTypes();

        var failingTypes = new List<Type>();
        foreach (Type entityType in entityTypes)
        {
            ConstructorInfo[] constructors = entityType
                .GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            if (constructors.Any())
            {
                failingTypes.Add(entityType);
            }
        }

        failingTypes.Should().BeEmpty();
    }
}
