using Application.Contracts.Persistence;
using Application.Features.Leads.Queries.GetLeadById;
using Application.Features.Leads.Queries.Shared;
using Application.Tests.Utils.Factories;
using Core.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Shared.Results;
using Tests.Common.ObjectMothers.Core;
using Xunit;

namespace Application.Tests.Leads.Queries.GetLeadById;

public sealed class GetLeadByIdQueryHandlerTests : IAsyncDisposable, IDisposable
{
    private readonly GetLeadByIdQueryHandler _handler;
    private readonly ILeadManagerDbContext _dbContext;
    private readonly CancellationTokenSource _cts;
    private readonly Lead _xptoIncLead;

    public GetLeadByIdQueryHandlerTests()
    {
        _xptoIncLead = LeadMother.XptoLLC();
        _cts = new();
        _dbContext = InMemoryLeadManagerDbContextFactory.Create();
        _dbContext.Leads.Add(_xptoIncLead);
        _dbContext.SaveChangesAsync().GetAwaiter().GetResult();
        _handler = new(_dbContext);
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
        _cts.Dispose();
        await _dbContext.Leads.ExecuteDeleteAsync();
        await _dbContext.DisposeAsync();
    }

    public void Dispose()
    {
        _cts.Dispose();
    }
}