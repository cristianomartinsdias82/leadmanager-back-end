using Application.Core.Processors;
using CrossCutting.Security.IAM;
using FluentAssertions;
using MediatR;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Shared.ApplicationOperationRules;
using Shared.Results;
using Xunit;

namespace Application.Tests.Core.Processors;

public sealed class ApplicationOperatingRulesProcessorTests
{
	[Fact]
	public async Task Processor_Process_ShouldCheckForUserAdministratorsMembership()
	{
		//Arrange
		var userServiceMock = Substitute.For<IUserService>();
		var sut = new ApplicationOperatingRulesProcessor<Unit>(userServiceMock, default!);

		//Act
		await sut.Process(Unit.Value, default);

		//Assert
		_ = userServiceMock.Received(1).CurrentUserIsAdministrator; //https://nsubstitute.github.io/help/received-calls/#checking-calls-to-properties
	}

	[Fact]
	public async Task Process_UserIsAdministrator_ShouldNotRunRulesEvaluations()
	{
		//Arrange
		using var cts = new CancellationTokenSource();
		var userServiceMock = Substitute.For<IUserService>();
		var ruleMock = Substitute.For<IApplicationOperatingRule>();
		ruleMock.ApplyAsync(Arg.Any<CancellationToken>()).Returns(default(Inconsistency?));
		ruleMock.Apply().Returns(default(Inconsistency?));
		userServiceMock.CurrentUserIsAdministrator.Returns(true);

		var sut = new ApplicationOperatingRulesProcessor<Unit>(userServiceMock, [ruleMock]);

		//Act
		await sut.Process(Unit.Value, cts.Token);

		//Assert
		await ruleMock.DidNotReceive().ApplyAsync(cts.Token);
		ruleMock.DidNotReceive().Apply();
	}

	[Fact]
	public async Task Process_UserIsNotAnAdministrator_ShouldRunRulesVerification()
	{
		//Arrange
		using var cts = new CancellationTokenSource();
		var userServiceMock = Substitute.For<IUserService>();
		
		var ruleMock = Substitute.For<IApplicationOperatingRule>();
		ruleMock.ApplyAsync(Arg.Any<CancellationToken>()).Returns(default(Inconsistency?));
		ruleMock.Apply().Returns(default(Inconsistency?));
		
		var ruleMock2 = Substitute.For<IApplicationOperatingRule>();
		ruleMock2.ApplyAsync(Arg.Any<CancellationToken>()).Returns(default(Inconsistency?));
		ruleMock2.Apply().Returns(default(Inconsistency?));
		userServiceMock.CurrentUserIsAdministrator.Returns(false);
		var applyExecutionCount = 0;

		var sut = new ApplicationOperatingRulesProcessor<Unit>(userServiceMock, [ruleMock, ruleMock2]);

		//Act
		await sut.Process(Unit.Value, cts.Token);

		//Assert
		try
		{
			await ruleMock.Received(1).ApplyAsync(cts.Token);
			++applyExecutionCount;

			await ruleMock2.Received(1).ApplyAsync(cts.Token);
			++applyExecutionCount;
		}
		catch (Exception) { }

		try
		{
			ruleMock.Received(1).Apply();
			++applyExecutionCount;

			ruleMock2.Received(1).Apply();
			++applyExecutionCount;
		}
		catch (Exception) { }
		
		applyExecutionCount.Should().BeGreaterThan(0);
	}
}
