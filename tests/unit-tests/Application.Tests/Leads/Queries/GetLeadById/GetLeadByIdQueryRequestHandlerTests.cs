using Application.Contracts.Persistence;
using Application.Features.Leads.Queries.GetLeadById;
using Application.Features.Leads.Shared;
using Core.Entities;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shared.Results;
using Shared.Tests;
using Tests.Common.ObjectMothers.Core;
using Xunit;

namespace Application.Tests.Leads.Queries.GetLeadById;

public sealed class GetLeadByIdQueryRequestHandlerTests : IAsyncDisposable, IDisposable
{
    private readonly GetLeadByIdQueryRequestHandler _handler;
    private readonly IMediator _mediator;
    private readonly ILeadManagerDbContext _dbContext;
    private readonly Lead _xptoIncLead;
    private readonly CancellationTokenSource _cts;

    public GetLeadByIdQueryRequestHandlerTests()
    {
        _xptoIncLead = LeadMother.XptoLLC();
        _dbContext = InMemoryLeadManagerDbContextFactory.Create();
        _dbContext.Leads.Add(_xptoIncLead);
        _dbContext.SaveChangesAsync().GetAwaiter().GetResult();
        _mediator = Substitute.For<IMediator>();
        _handler = new(_mediator, _dbContext);
        _cts = new();
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

    public async ValueTask DisposeAsync()
    {
        await _dbContext.Leads.ExecuteDeleteAsync();
        await _dbContext.DisposeAsync();
    }

    public void Dispose()
    {
        _cts.Dispose();
    }
}