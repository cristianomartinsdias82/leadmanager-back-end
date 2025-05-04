namespace Application.Security;

public static class LeadManagerSecurityConfiguration
{
    public static class Policies
    {
        public const string LeadManagerDefaultPolicy = "LeadManagerDefaultPolicy";
        public const string LeadManagerRemovePolicy = "LeadManagerRemovePolicy";
        public const string LeadManagerCorsPolicy = "LeadWebAppCorsPolicy";
        public const string LeadManagerAdministrativeTasksPolicy = "LeadManagerAdministrativeTasksPolicy";
    }

    public static class Roles
    {
        public const string Administrators = "Administrators";
    }

    public static class ClaimTypes
    {
        public const string LDM = "ldm";
    }

    public static class Claims
    {
        public const string Read = "leadmanager.read";
        public const string Insert = "leadmanager.insert";
        public const string BulkInsert = "leadmanager.bulk_insert";
        public const string Update = "leadmanager.update";
        public const string Delete = "leadmanager.delete";
    }

    public static class Permissions
    {
        public const string Read = Claims.Read;
        public const string Insert = Claims.Insert;
        public const string BulkInsert = Claims.BulkInsert;
        public const string Update = Claims.Update;
        public const string Delete = Claims.Delete;
    }
}
