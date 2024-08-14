namespace Infrastructure.Persistence.Mappings;

public static class AuditEntryMetadata
{
    public static string DatabaseSchemaName = "Auditing";
    public static string TableName = "AuditEntries";
    public static string AuditEntryIdColumnPkId = "PK_AuditEntry_Id";
}
