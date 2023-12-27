using Application.Core.Contracts.Repository.Security;
using Application.Security.OneTimePassword.Commands.GenerateOneTimePassword;
using Application.Security.OneTimePassword.Commands.HandleOneTimePassword;
using CrossCutting.Security.IAM;
using CrossCutting.Security.Secrecy;
using FluentAssertions;
using MediatR;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Xunit;

namespace Application.Tests.Security.OneTimePassword.Commands.GenerateOneTimePassword;

public sealed class GenerateOneTimePasswordCommandRequestHandlerTests
{
    [Fact]
    public async Task Handle_GenerateOneTimePassword_ReturnsSuccessfully()
    {
        //Arrange
        const string FakeCode = "123456";
        var FakeUserId = Guid.NewGuid();

        using var cts = new CancellationTokenSource();
        var mediator = Substitute.For<IMediator>();
        var userService = Substitute.For<IUserService>();
        userService.GetUserId().Returns(FakeUserId);
        userService.GetUserId()!.Value.Returns(FakeUserId);
        var secretGeneratorService = Substitute.For<ISecretGenerationService>();
        secretGeneratorService.GenerateAsync(
                                    Arg.Any<int>(),
                                    Arg.Any<int>(),
                                    Arg.Any<bool>(),
                                    Arg.Any<bool>(),
                                    Arg.Any<bool>(),
                                    Arg.Any<bool>(),
                                    cancellationToken: cts.Token)
                                .Returns(FakeCode);

        var oneTimePasswordRepository = Substitute.For<IOneTimePasswordRepository>();

        //Act
        var result = await new GenerateOneTimePasswordCommandRequestHandler(
                                                mediator,
                                                userService,
                                                secretGeneratorService,
                                                oneTimePasswordRepository)
                                            .Handle(new() { Resource = string.Empty }, cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().BeOfType<GenerateOneTimePasswordCommandResponse>();
        result.Data.OneTimePassword.Should().NotBeNullOrWhiteSpace().And.Be(FakeCode);
        userService.Received(1).GetUserId();
        await secretGeneratorService
                .Received(1)
                .GenerateAsync(
                    Arg.Any<int>(),
                    Arg.Any<int>(),
                    Arg.Any<bool>(),
                    Arg.Any<bool>(),
                    Arg.Any<bool>(),
                    Arg.Any<bool>(),
                    cts.Token);
        await oneTimePasswordRepository.Received(1).SaveAsync(Arg.Any<OneTimePasswordDto>(), cts.Token);
    }
}