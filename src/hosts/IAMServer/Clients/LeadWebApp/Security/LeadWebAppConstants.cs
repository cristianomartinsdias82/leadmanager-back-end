namespace IAMServer.Clients.LeadWebApp.Security;

public static class LeadManagerAppConstants
{
    public const string CorsPolicyName = "LeadWebAppCorsPolicy";

    public static class Claims
    {
        public const string LDM = "ldm";
        public const string Read = "leadmanager.read";
        public const string Insert = "leadmanager.insert";
        public const string BulkInsert = "leadmanager.bulk_insert";
        public const string Update = "leadmanager.update";
        public const string Delete = "leadmanager.delete";
    }

    public static class Roles
    {
        public const string Administrators = "Administrators";
    }
}
