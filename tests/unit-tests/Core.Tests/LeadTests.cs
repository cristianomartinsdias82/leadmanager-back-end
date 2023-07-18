using Core.BusinessExceptions;
using Core.Entities;
using FluentAssertions;
using Tests.Common.ObjectMothers.Core;
using Xunit;

namespace Core.Tests;

public sealed class LeadTests
{
    [Fact]
    public void Constructor_AllArgumentsAreValid_RunsSuccessfully()
    {
        //Arrange
        var leadDraft = new
        {
            Cnpj = CnpjMother.MaskedWellformedValidOne(),
            RazaoSocial = "Gumper Inc.",
            Cep = "04661-100",
            Endereco = "Rua das Piracicabas",
            Cidade = "Nova Odessa",
            Bairro = "Vila Mussum",
            Estado = "SP",
            Numero = "176",
            Complemento = "Casa IV"
        };
        var newlyCreatedLead = default(Lead);

        //Act
        var createLead = () =>
        {
            newlyCreatedLead = new(
                leadDraft.Cnpj,
                leadDraft.RazaoSocial,
                leadDraft.Cep,
                leadDraft.Endereco,
                leadDraft.Bairro,
                leadDraft.Cidade,
                leadDraft.Estado,
                leadDraft.Numero,
                leadDraft.Complemento);
        };

        //Assert
        createLead.Should()
                .NotThrow<BusinessException>()
            .And.NotThrow<InvalidAddressException>()
            .And.NotThrow<InvalidNationalTaxIdentificationNumberException>()
            .And.NotThrow<InvalidZipCodeException>();
        newlyCreatedLead.Should().NotBeNull();
        newlyCreatedLead!.Cnpj.Should().NotBeNull().And.BeEquivalentTo(leadDraft.Cnpj);
        newlyCreatedLead!.RazaoSocial.Should().NotBeNull().And.BeEquivalentTo(leadDraft.RazaoSocial);
        newlyCreatedLead!.Cep.Should().NotBeNull().And.BeEquivalentTo(leadDraft.Cep);
        newlyCreatedLead!.Logradouro.Should().NotBeNull().And.BeEquivalentTo(leadDraft.Endereco);
        newlyCreatedLead!.Bairro.Should().NotBeNull().And.BeEquivalentTo(leadDraft.Bairro);
        newlyCreatedLead!.Cidade.Should().NotBeNull().And.BeEquivalentTo(leadDraft.Cidade);
        newlyCreatedLead!.Estado.Should().NotBeNull().And.BeEquivalentTo(leadDraft.Estado);
        newlyCreatedLead!.Numero.Should().NotBeNull().And.BeEquivalentTo(leadDraft.Numero);
        newlyCreatedLead!.Complemento.Should().NotBeNull().And.BeEquivalentTo(leadDraft.Complemento);
    }

    [Theory]
    [MemberData(nameof(InvalidCnpjSimulations))]
    public void Constructor_CnpjArgumentIsInvalid_ThrowsInvalidNationalTaxIdentificationNumberException(string cnpj)
    {
        //Arrange
        var leadDraft = new
        {
            Cnpj = cnpj,
            RazaoSocial = "Gumper Inc.",
            Cep = CepMother.MaskedWellformedValidOne(),
            Endereco = "Rua das Piracicabas",
            Cidade = "Nova Odessa",
            Bairro = "Vila Mussum",
            Estado = "SP",
            Numero = "176",
            Complemento = "Casa IV"
        };
        var newlyCreatedLead = default(Lead);

        //Act
        var createLead = () =>
        {
            newlyCreatedLead = new(
                leadDraft.Cnpj,
                leadDraft.RazaoSocial,
                leadDraft.Cep,
                leadDraft.Endereco,
                leadDraft.Bairro,
                leadDraft.Cidade,
                leadDraft.Estado,
                leadDraft.Numero,
                leadDraft.Complemento);
        };

        //Assert
        createLead.Should().Throw<BusinessException>();
        createLead.Should().Throw<InvalidNationalTaxIdentificationNumberException>();
    }

    [Theory]
    [MemberData(nameof(InvalidZipCodeSimulations))]
    public void Constructor_ZipCodeArgumentIsInvalid_ThrowsInvalidZipCodeException(string zipCode)
    {
        //Arrange
        var leadDraft = new
        {
            Cnpj = CnpjMother.MaskedWellformedValidOne(),
            RazaoSocial = "Gumper Inc.",
            Cep = zipCode,
            Endereco = "Rua das Piracicabas",
            Cidade = "Nova Odessa",
            Bairro = "Vila Mussum",
            Estado = "SP",
            Numero = "176",
            Complemento = "Casa IV"
        };
        var newlyCreatedLead = default(Lead);

        //Act
        var createLead = () =>
        {
            newlyCreatedLead = new(
                leadDraft.Cnpj,
                leadDraft.RazaoSocial,
                leadDraft.Cep,
                leadDraft.Endereco,
                leadDraft.Bairro,
                leadDraft.Cidade,
                leadDraft.Estado,
                leadDraft.Numero,
                leadDraft.Complemento);
        };

        //Assert
        createLead.Should().Throw<BusinessException>();
        createLead.Should().Throw<InvalidZipCodeException>();
    }

    [Fact]
    public void Atualizar_AllArgumentsAreValid_RunsSuccessfully()
    {
        //Arrange
        var leadUpdateDraft = new
        {
            Cnpj = "63.167.653/0001-80",
            RazaoSocial = "Xaxim Ltda-Me",
            Cep = "04661-100",
            Endereco = "Rua das Piracicabas",
            Cidade = "Nova Odessa",
            Bairro = "Vila Mussum",
            Estado = "SP",
            Numero = "176",
            Complemento = "Casa IV"
        };
        Lead lead = LeadMother.XptoLLC();

        //Act
        var updateLead = () =>
        {
            lead.Atualizar(
                leadUpdateDraft.RazaoSocial,
                leadUpdateDraft.Cnpj,
                leadUpdateDraft.Cep,
                leadUpdateDraft.Endereco,
                leadUpdateDraft.Cidade,
                leadUpdateDraft.Estado,
                leadUpdateDraft.Bairro,
                leadUpdateDraft.Numero,
                leadUpdateDraft.Complemento);
        };

        //Assert
        updateLead.Should()
                .NotThrow<BusinessException>()
            .And.NotThrow<InvalidAddressException>()
            .And.NotThrow<InvalidNationalTaxIdentificationNumberException>()
            .And.NotThrow<InvalidZipCodeException>();
        lead.Should().NotBeNull();
        lead!.Cnpj.Should().NotBeNull().And.BeEquivalentTo(leadUpdateDraft.Cnpj);
        lead!.RazaoSocial.Should().NotBeNull().And.BeEquivalentTo(leadUpdateDraft.RazaoSocial);
        lead!.Cep.Should().NotBeNull().And.BeEquivalentTo(leadUpdateDraft.Cep);
        lead!.Logradouro.Should().NotBeNull().And.BeEquivalentTo(leadUpdateDraft.Endereco);
        lead!.Bairro.Should().NotBeNull().And.BeEquivalentTo(leadUpdateDraft.Bairro);
        lead!.Cidade.Should().NotBeNull().And.BeEquivalentTo(leadUpdateDraft.Cidade);
        lead!.Estado.Should().NotBeNull().And.BeEquivalentTo(leadUpdateDraft.Estado);
        lead!.Numero.Should().NotBeNull().And.BeEquivalentTo(leadUpdateDraft.Numero);
        lead!.Complemento.Should().NotBeNull().And.BeEquivalentTo(leadUpdateDraft.Complemento);
    }

    [Theory]
    [MemberData(nameof(InvalidCnpjSimulations))]
    public void Atualizar_CnpjArgumentIsInvalid_ThrowsInvalidNationalTaxIdentificationNumberException(string cnpj)
    {
        //Arrange
        var leadUpdateDraft = new
        {
            Cnpj = cnpj,
            RazaoSocial = "Xaxim Ltda-Me",
            Cep = "04661-100",
            Endereco = "Rua das Piracicabas",
            Cidade = "Nova Odessa",
            Bairro = "Vila Mussum",
            Estado = "SP",
            Numero = "176",
            Complemento = "Casa IV"
        };
        Lead lead = LeadMother.XptoLLC();

        //Act
        var updateLead = () =>
        {
            lead.Atualizar(
                leadUpdateDraft.RazaoSocial,
                leadUpdateDraft.Cnpj,
                leadUpdateDraft.Cep,
                leadUpdateDraft.Endereco,
                leadUpdateDraft.Cidade,
                leadUpdateDraft.Estado,
                leadUpdateDraft.Bairro,
                leadUpdateDraft.Numero,
                leadUpdateDraft.Complemento);
        };

        //Assert
        updateLead.Should().Throw<BusinessException>();
        updateLead.Should().Throw<InvalidNationalTaxIdentificationNumberException>();
    }

    [Theory]
    [MemberData(nameof(InvalidZipCodeSimulations))]
    public void Atualizar_ZipCodeArgumentIsInvalid_ThrowsInvalidZipCodeException(string zipCode)
    {
        var leadUpdateDraft = new
        {
            Cnpj = CnpjMother.MaskedWellformedValidOne(),
            RazaoSocial = "Xaxim Ltda-Me",
            Cep = zipCode,
            Endereco = "Rua das Piracicabas",
            Cidade = "Nova Odessa",
            Bairro = "Vila Mussum",
            Estado = "SP",
            Numero = "176",
            Complemento = "Casa IV"
        };
        Lead lead = LeadMother.XptoLLC();

        //Act
        var updateLead = () =>
        {
            lead.Atualizar(
                leadUpdateDraft.RazaoSocial,
                leadUpdateDraft.Cnpj,
                leadUpdateDraft.Cep,
                leadUpdateDraft.Endereco,
                leadUpdateDraft.Cidade,
                leadUpdateDraft.Estado,
                leadUpdateDraft.Bairro,
                leadUpdateDraft.Numero,
                leadUpdateDraft.Complemento);
        };

        //Assert
        updateLead.Should().Throw<BusinessException>();
        updateLead.Should().Throw<InvalidZipCodeException>();
    }

    public static IEnumerable<object[]> InvalidCnpjSimulations()
    {
        yield return new object[] { CnpjMother.MaskedWellformedInvalidOne() };
        yield return new object[] { CnpjMother.MaskedMalformedValidOne() };
        yield return new object[] { CnpjMother.MaskedMalformedInvalidOne() };
    }

    public static IEnumerable<object[]> InvalidZipCodeSimulations()
    {
        yield return new object[] { CepMother.MaskedMalformedValidOne() };
        yield return new object[] { CepMother.MaskedMalformedInvalidOne() };
    }
}