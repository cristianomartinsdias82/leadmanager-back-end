using Application.Core.Contracts.Repository.Prospecting;
using Application.Prospecting.Leads.Queries.GetLeadById;
using CrossCutting.Security.IAM;
using Domain.Prospecting.Entities;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Shared.Results;
using Tests.Common.ObjectMothers.Domain;
using Xunit;

namespace Application.Tests.Prospecting.Leads.Queries.GetLeadById;

public sealed class GetLeadByIdQueryRequestHandlerTests
{
    private readonly GetLeadByIdQueryRequestHandler _handler;
    private readonly IUserService _userService;
    private readonly IMediator _mediator;
    private readonly ILeadRepository _leadRepositoryMock;
    private readonly CancellationTokenSource _cts;

    public GetLeadByIdQueryRequestHandlerTests()
    {
        _userService = Substitute.For<IUserService>();
        _userService.GetUserId().Returns(Guid.NewGuid());
        _leadRepositoryMock = Substitute.For<ILeadRepository>();
        
        _mediator = Substitute.For<IMediator>();
        _handler = new(_mediator, _leadRepositoryMock);
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
        var _xptoIncLead = LeadMother.XptoLLC();
        _leadRepositoryMock.GetByIdAsync(_xptoIncLead.Id, _cts.Token).Returns(_xptoIncLead);
        GetLeadByIdQueryRequest request = new() { Id = _xptoIncLead.Id };

        //Act
        var result = await _handler.Handle(request, _cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ApplicationResponse<LeadDto>>();
        result.OperationCode.Should().Be(OperationCodes.Successful);
        result.Exception.Should().BeNull();
        result.Data.Should().NotBeNull();
        result.Data.Id.Should().Be(_xptoIncLead.Id);
        result.Data.RazaoSocial.Should().Be(_xptoIncLead.RazaoSocial);
        result.Data.Cnpj.Should().Be(_xptoIncLead.Cnpj);
    }
}