using Application.Core.Contracts.Repository.Caching;
using Application.Core.Contracts.Repository.Security;
using Application.Security.OneTimePassword.Commands.GenerateOneTimePassword;
using Application.Security.OneTimePassword.Commands.HandleOneTimePassword;
using CrossCutting.EndUserCommunication.Sms;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Shared.Results;
using Xunit;

namespace Application.Tests.Prospecting.Security.OneTimePassword.Commands.HandleOneTimePassword;

public sealed class HandleOneTimePasswordCommandRequestHandlerTests
{
    private const string ExpectedGeneratedCode = "739456";
    private const string Resource = "xpto";
    private const int TtlInSeconds = 86400;
    private const int ExpirationTimeInSeconds = 10;
    private const int ExpirationTimeOffsetInSeconds = 10;

    private readonly HandleOneTimePasswordCommandRequestHandler _handler;
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly OneTimePasswordCachingPolicy _oneTimePasswordCachingPolicy = new(ExpirationTimeInSeconds, ExpirationTimeOffsetInSeconds, TtlInSeconds);
    private readonly IOneTimePasswordRepository _oneTimePasswordRepository = Substitute.For<IOneTimePasswordRepository>();
    private readonly ISmsDeliveryService _smsDeliveryService = Substitute.For<ISmsDeliveryService>();
    private readonly CancellationTokenSource _cts = new();

    public HandleOneTimePasswordCommandRequestHandlerTests()
    {
        _handler = new(_oneTimePasswordCachingPolicy, _mediator, _oneTimePasswordRepository, _smsDeliveryService);
    }

    ~HandleOneTimePasswordCommandRequestHandlerTests()
    {
        _cts.Dispose();
    }

    [Fact(Skip = "This scenario is not testable: IMediator is used inside passing an entirely new parameter that is not controlled from the outside and therefore cannot not Mocked.")]
    public async Task Handle_OtpNotInformed_GeneratesAndStores_And_Returns_CodeGeneratedSuccessfully()
    {
        //Arrange
        var request = new HandleOneTimePasswordCommandRequest
        {
            Resource = string.Empty,
            UserId = Guid.NewGuid()
        };
        _mediator.Send(Arg.Any<GenerateOneTimePasswordCommandRequest>(), _cts.Token)
                .Returns(ApplicationResponse<GenerateOneTimePasswordCommandResponse>.Create(new(ExpectedGeneratedCode), operationCode: OperationCodes.Successful));
        _smsDeliveryService.SendAsync(Arg.Any<string>(), _cts.Token)
                           .Returns(Task.CompletedTask);

        //Act
        var result = await _handler.Handle(request, _cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().BeOfType<HandleOneTimePasswordCommandResponse>();
        result.Data.Result.Should().Be(OneTimePasswordHandlingOperationResult.CodeGeneratedSuccessfully);
        await _oneTimePasswordRepository.Received(1).SaveAsync(Arg.Any<OneTimePasswordDto>(), _cts.Token);
        await _smsDeliveryService
                .Received(1)
                .SendAsync(Arg.Any<string>(), _cts.Token);
    }

    [Fact]
    public async Task Handle_WrongOtpInformed_Returns_InvalidCode()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var request = new HandleOneTimePasswordCommandRequest
        {
            Resource = Resource,
            UserId = userId,
            Code = "9999999"
        };
        var oneTimePasswordDto = new OneTimePasswordDto
        (
            userId,
            Resource,
            DateTime.UtcNow,
            ExpectedGeneratedCode
        );
        _oneTimePasswordRepository
            .GetAsync(userId, Resource, _cts.Token)
            .Returns(oneTimePasswordDto);
        _oneTimePasswordRepository
            .RemoveAsync(userId, Resource, _cts.Token)
            .Returns(Task.CompletedTask);

        //Act
        var result = await _handler.Handle(request, _cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().BeOfType<HandleOneTimePasswordCommandResponse>();
        result.Data.Result.Should().Be(OneTimePasswordHandlingOperationResult.InvalidCode);
        await _oneTimePasswordRepository
                .Received(1)
                .GetAsync(userId, Resource, _cts.Token);
        await _oneTimePasswordRepository
                .Received(0)
                .RemoveAsync(userId, Resource, _cts.Token);
        await _oneTimePasswordRepository
                .Received(0)
                .SaveAsync(Arg.Any<OneTimePasswordDto>(), _cts.Token);
        await _smsDeliveryService
                .Received(0)
                .SendAsync(Arg.Any<string>(), _cts.Token);
    }

    [Theory]
    [InlineData(ExpectedGeneratedCode)]
    [InlineData("375884")]
    public async Task Handle_OtpExpiredInformed_Returns_ExpiredCode(string code)
    {
        //Arrange
        var userId = Guid.NewGuid();
        var request = new HandleOneTimePasswordCommandRequest
        {
            Resource = Resource,
            UserId = userId,
            Code = code
        };
        var oneTimePasswordDto = new OneTimePasswordDto
        (
            userId,
            Resource,
            DateTime.UtcNow
                        .AddSeconds(-_oneTimePasswordCachingPolicy.ExpirationTimeInSeconds)
                        .AddSeconds(-_oneTimePasswordCachingPolicy.ExpirationTimeOffsetInSeconds)
                        .AddMilliseconds(-1),
            code
        );
        _oneTimePasswordRepository
            .GetAsync(userId, Resource, _cts.Token)
            .Returns(oneTimePasswordDto);

        //Act
        var result = await _handler.Handle(request, _cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().BeOfType<HandleOneTimePasswordCommandResponse>();
        result.Data.Result.Should().Be(OneTimePasswordHandlingOperationResult.ExpiredCode);
        await _oneTimePasswordRepository
                .Received(1)
                .GetAsync(userId, Resource, _cts.Token);
        await _oneTimePasswordRepository
                .Received(0)
                .RemoveAsync(userId, Resource, _cts.Token);
        await _oneTimePasswordRepository
                .Received(0)
                .SaveAsync(Arg.Any<OneTimePasswordDto>(), _cts.Token);
        await _smsDeliveryService
                .Received(0)
                .SendAsync(Arg.Any<string>(), _cts.Token);
    }

    [Fact]
    public async Task Handle_ValidOtp_Return_ValidCode()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var request = new HandleOneTimePasswordCommandRequest
        {
            Resource = Resource,
            UserId = userId,
            Code = ExpectedGeneratedCode
        };
        var oneTimePasswordDto = new OneTimePasswordDto
        (
            userId,
            Resource,
            DateTime.UtcNow,
            ExpectedGeneratedCode
        );
        _oneTimePasswordRepository
            .GetAsync(userId, Resource, _cts.Token)
            .Returns(oneTimePasswordDto);

        //Act
        var result = await _handler.Handle(request, _cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().BeOfType<HandleOneTimePasswordCommandResponse>();
        result.Data.Result.Should().Be(OneTimePasswordHandlingOperationResult.ValidCode);
        await _oneTimePasswordRepository
                .Received(1)
                .GetAsync(userId, Resource, _cts.Token);
        await _oneTimePasswordRepository
                .Received(1)
                .RemoveAsync(userId, Resource, _cts.Token);
        await _oneTimePasswordRepository
                .Received(0)
                .SaveAsync(Arg.Any<OneTimePasswordDto>(), _cts.Token);
        await _smsDeliveryService
                .Received(0)
                .SendAsync(Arg.Any<string>(), _cts.Token);
    }
}
