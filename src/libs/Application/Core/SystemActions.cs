using System.ComponentModel;

namespace Application.Core;

public enum SystemActions
{
    [Description("Acesso ao sistema")]
    Login,
    [Description("Registro de Lead")]
    LeadRegistration,
    [Description("Registro de Lead em lote")]
    BulkLeadRegistration,
    [Description("Atualização de dados de Lead")]
    LeadDataUpdate,
    [Description("Exclusão de Lead")]
    LeadExclusion,
}
