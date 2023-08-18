using Application.Contracts.Persistence;
using Application.Features.Leads.Commands.UpdateLead;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shared.Events.EventDispatching;
using Shared.Results;
using Shared.Tests;
using Tests.Common.ObjectMothers.Application;
using Tests.Common.ObjectMothers.Core;
using Xunit;

namespace Application.Tests.Leads.Commands.UpdateLead;

public sealed class UpdateLeadCommandHandlerTests : IAsyncDisposable, IDisposable
{
    private readonly UpdateLeadCommandHandler _handler;
    private readonly ILeadManagerDbContext _dbContext;
    private readonly IMediator _mediator;
    private readonly IEventDispatching _eventDispatcher;
    private readonly CancellationTokenSource _cts;    

    public UpdateLeadCommandHandlerTests()
    {
        _dbContext = InMemoryLeadManagerDbContextFactory.Create();
        _mediator = Substitute.For<IMediator>();
        _eventDispatcher = Substitute.For<IEventDispatching>();
        _handler = new UpdateLeadCommandHandler(_mediator, _eventDispatcher, _dbContext);
        _cts = new();
    }

    public void Dispose()
    {
        _cts.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _dbContext.Leads.ExecuteDeleteAsync();
    }

    [Fact]
    public async Task Handle_ValidRequestParametersAndExistingLead_ShouldSucceed()
    {
        //Arrange
        var leadId = Guid.NewGuid();
        var request = UpdateLeadCommandRequestMother
                        .Instance
                        .WithId(leadId)
                        .WithRazaoSocial("XPTO Brasil LLC")
                        .WithCep("11111-111")
                        .WithEndereco("Avenida Carlos Ribeiro")
                        .WithCidade("Aracaju")
                        .WithEstado("SE")
                        .Build();
        var lead = LeadMother.XptoLLC();
        lead.Id = leadId;
        
        await _dbContext.Leads.AddAsync(lead, _cts.Token);
        await _dbContext.SaveChangesAsync(_cts.Token);

        //Act
        var result = await _handler.Handle(request, _cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.OperationCode.Should().Be(OperationCodes.Successful);
        result.Success.Should().BeTrue();
        result.Inconsistencies.Should().BeNullOrEmpty();
        result.Should().BeOfType<ApplicationResponse<UpdateLeadCommandResponse>>();
        var updatedLead = await _dbContext.Leads.FindAsync(request.Id, _cts.Token);
        updatedLead.Should().NotBeNull();
        updatedLead!.Cep.Should().Be(request.Cep);
        updatedLead.Logradouro.Should().Be(request.Endereco);
        updatedLead.Cidade.Should().Be(request.Cidade);
        updatedLead.Estado.Should().Be(request.Estado);
    }

    [Fact]
    public async Task Handle_ValidRequestParametersWithNonExistingLead_ShouldReturnResultObjectWithNotFoundMessage()
    {
        //Arrange
        var request = UpdateLeadCommandRequestMother
                        .Instance
                        .WithId(Guid.NewGuid())
                        .WithRazaoSocial("XPTO Brasil LLC")
                        .WithCep("11111-111")
                        .WithEndereco("Avenida Carlos Ribeiro")
                        .WithCidade("Salto")
                        .WithEstado("SP")
                        .Build();
        var lead = LeadMother.XptoLLC();

        await _dbContext.Leads.AddAsync(lead, _cts.Token);
        await _dbContext.SaveChangesAsync(_cts.Token);

        //Act
        var result = await _handler.Handle(request, _cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.OperationCode.Should().Be(OperationCodes.Successful);
        result.Success.Should().BeTrue();
        result.Inconsistencies.Should().BeNullOrEmpty();
        result.Should().BeOfType<ApplicationResponse<UpdateLeadCommandResponse>>();
        result.Message.Should().BeEquivalentTo("Lead não encontrado.");
    }
}
