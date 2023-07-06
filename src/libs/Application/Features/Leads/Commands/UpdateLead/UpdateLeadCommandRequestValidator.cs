﻿using FluentValidation;
using Shared.Validation;

namespace Application.Features.Leads.Commands.UpdateLead;

public sealed class UpdateLeadCommandRequestValidator : AbstractValidator<UpdateLeadCommandRequest>
{
    public UpdateLeadCommandRequestValidator()
    {
        RuleFor(lead => lead)
            .NotNull()
            .WithMessage("Requisição inválida.");

        RuleFor(lead => lead.Id)
            .NotEmpty()
            .WithMessage("Campo Id é obrigatório.");

        RuleFor(lead => lead.Cnpj)
            .NotEmpty()
            .WithMessage("Campo Cnpj é obrigatório.");

        When(lead => !string.IsNullOrWhiteSpace(lead.Cnpj), () => {
            RuleFor(lead => lead.Cnpj)
            .IsValidCnpj()
            .WithMessage("Campo Cnpj é inválido.");
        });

        RuleFor(lead => lead.Cep)
            .NotEmpty()
            .WithMessage("Campo Cep é obrigatório.");

        RuleFor(lead => lead.Endereco)
            .NotEmpty()
            .WithMessage("Campo Endereço é obrigatório.");

        RuleFor(lead => lead.Bairro)
            .NotEmpty()
            .WithMessage("Campo Bairro é obrigatório.");

        RuleFor(lead => lead.Cidade)
            .NotEmpty()
            .WithMessage("Campo Cidade é obrigatório.");

        RuleFor(lead => lead.Estado)
            .NotEmpty()
            .WithMessage("Campo Estado é obrigatório.")
            .Length(2)
            .WithMessage("Campo Estado deve conter 2 caracteres.");

        //When(lead => !string.IsNullOrWhiteSpace(lead.Numero),
        //    () =>
        //    {

        //    });
    }
}