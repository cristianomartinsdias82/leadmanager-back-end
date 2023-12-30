namespace Domain.Prospecting.Entities;

public interface ILead
{
    string Cnpj { get; }
    string RazaoSocial { get; }
}