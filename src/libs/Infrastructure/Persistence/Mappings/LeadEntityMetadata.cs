namespace Infrastructure.Persistence.Mappings;

public static class LeadEntityMetadata
{
    public static string DatabaseSchemaName = "Prospecting";
    public static string TableName = "Leads";
    public static string CnpjColumnIndexName = "IX_Leads_Cnpj";
    public static string RazaoSocialColumnIndexName = "IX_Leads_RazaoSocial";
    public static string LeadIdColumnPkId = "PK_Lead_Id";
}
