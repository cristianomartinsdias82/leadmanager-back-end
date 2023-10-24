using Application.Core.Contracts.Persistence;
using Application.Prospecting.Leads.Queries.GetLeadById;
using Application.Prospecting.Leads.Shared;
using CrossCutting.Security.IAM;
using Domain.Prospecting.Entities;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Shared.Results;
using Tests.Common.Factories;
using Tests.Common.ObjectMothers.Domain;
using Xunit;

namespace Application.Tests.Prospecting.Leads.Queries.GetLeadById;

public sealed class GetLeadByIdQueryRequestHandlerTests : IAsyncDisposable
{
    private readonly GetLeadByIdQueryRequestHandler _handler;
    private readonly IUserService _userService;
    private readonly IMediator _mediator;
    private readonly ILeadManagerDbContext _dbContext;
    private readonly Lead _xptoIncLead;
    private readonly CancellationTokenSource _cts;

    public GetLeadByIdQueryRequestHandlerTests()
    {
        _userService = Substitute.For<IUserService>();
        _userService.GetUserId().Returns(Guid.NewGuid());
        _dbContext = InMemoryLeadManagerDbContextFactory.Create(_userService);
        _xptoIncLead = LeadMother.XptoLLC();
        _dbContext.Leads.Add(_xptoIncLead);
        _dbContext.SaveChangesAsync().GetAwaiter().GetResult();
        _mediator = Substitute.For<IMediator>();
        _handler = new(_mediator, _dbContext);
        _cts = new();
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Dispose();
        //await _dbContext.Leads.ExecuteDeleteAsync();
        await _dbContext.DisposeAsync();
    }

    [Fact]
    public async void Handle_WhenNonExistingLeadInformed_ShouldReturnNotFound()
    {
        //Arrange
        GetLeadByIdQueryRequest request = new() { Id = Guid.NewGuid() };

        //Act
        var result = await _handler.Handle(request, _cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ApplicationResponse<LeadDto>>();
        result.Data.Should().BeNull();
        result.OperationCode.Should().Be(OperationCodes.NotFound);
        result.Exception.Should().BeNull();
    }

    [Fact]
    public async void Handle_WhenExistingLeadInformed_ShouldReturnSuccessfulWithLeadData()
    {
        //Arrange
        GetLeadByIdQueryRequest request = new() { Id = _xptoIncLead.Id };

        //Act
        var result = await _handler.Handle(request, _cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ApplicationResponse<LeadDto>>();
        result.Data.Should().NotBeNull();
        result.Data.Id.Should().Be(_xptoIncLead.Id);
        result.OperationCode.Should().Be(OperationCodes.Successful);
        result.Exception.Should().BeNull();
    }
}