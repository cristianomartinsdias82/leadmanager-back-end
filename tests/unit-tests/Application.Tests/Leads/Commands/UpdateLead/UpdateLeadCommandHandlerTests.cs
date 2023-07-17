using Application.Contracts.Persistence;
using Application.Features.Leads.Commands.UpdateLead;
using Application.Tests.Utils.Factories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Shared.Results;
using Tests.Common.ObjectMothers.Application;
using Tests.Common.ObjectMothers.Core;
using Xunit;

namespace Application.Tests.Leads.Commands.UpdateLead;

public sealed class UpdateLeadCommandHandlerTests : IAsyncDisposable
{
    private readonly ILeadManagerDbContext _dbContext;

    public UpdateLeadCommandHandlerTests()
    {
        _dbContext = InMemoryLeadManagerDbContextFactory.Create();
    }

    public async ValueTask DisposeAsync()
    {
        await _dbContext.Leads.ExecuteDeleteAsync();
    }

    [Fact]
    public async Task Handle_ValidRequestParametersAndExistingLead_ShouldSucceed()
    {
        //Arrange
        var handler = new UpdateLeadCommandHandler(_dbContext);
        using var cts = new CancellationTokenSource();
        var leadId = Guid.NewGuid();
        var request = UpdateLeadCommandRequestMother
                        .Instance
                        .WithId(leadId)
                        .WithCnpj(CnpjMother.MaskedWellformedValidOne())
                        .WithRazaoSocial("XPTO Brasil LLC")
                        .WithCep("11111-111")
                        .WithEndereco("Avenida Carlos Ribeiro")
                        .WithCidade("Aracaju")
                        .WithEstado("SE")
                        .Build();
        var lead = LeadMother.XptoLLC();
        lead.Id = leadId;
        
        await _dbContext.Leads.AddAsync(lead, cts.Token);
        await _dbContext.SaveChangesAsync(cts.Token);

        //Act
        var result = await handler.Handle(request, cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.OperationCode.Should().Be(OperationCodes.Successful);
        result.Success.Should().BeTrue();
        result.Inconsistencies.Should().BeNullOrEmpty();
        result.Should().BeOfType<ApplicationResponse<UpdateLeadCommandResponse>>();
        var updatedLead = await _dbContext.Leads.FindAsync(request.Id, cts.Token);
        updatedLead.Should().NotBeNull();
        updatedLead!.Cnpj.Should().Be(request.Cnpj);
        updatedLead.Cep.Should().Be(request.Cep);
        updatedLead.Logradouro.Should().Be(request.Endereco);
        updatedLead.Cidade.Should().Be(request.Cidade);
        updatedLead.Estado.Should().Be(request.Estado);
    }

    [Fact]
    public async Task Handle_ValidRequestParametersWithNonExistingLead_ShouldReturnResultObjectWithNotFoundMessage()
    {
        //Arrange
        var handler = new UpdateLeadCommandHandler(_dbContext);
        using var cts = new CancellationTokenSource();
        var request = UpdateLeadCommandRequestMother
                        .Instance
                        .WithId(Guid.NewGuid())
                        .WithCnpj("12.312.321/0001-23")
                        .WithRazaoSocial("XPTO Brasil LLC")
                        .WithCep("11111-111")
                        .WithEndereco("Avenida Carlos Ribeiro")
                        .WithCidade("Salto")
                        .WithEstado("SP")
                        .Build();
        var lead = LeadMother.XptoLLC();

        await _dbContext.Leads.AddAsync(lead, cts.Token);
        await _dbContext.SaveChangesAsync(cts.Token);

        //Act
        var result = await handler.Handle(request, cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.OperationCode.Should().Be(OperationCodes.Successful);
        result.Success.Should().BeTrue();
        result.Inconsistencies.Should().BeNullOrEmpty();
        result.Should().BeOfType<ApplicationResponse<UpdateLeadCommandResponse>>();
        result.Message.Should().BeEquivalentTo("Lead não encontrado.");
    }
}
