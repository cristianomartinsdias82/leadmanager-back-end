using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence.DesignTimeMigrationsHelper;

//https://go.microsoft.com/fwlink/?linkid=851728
//https://learn.microsoft.com/en-us/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli
public sealed class LeadManagerDbContextFactory : IDesignTimeDbContextFactory<LeadManagerDbContext>
{
    public LeadManagerDbContext CreateDbContext(string[] args)
    {
        Console.WriteLine($"args = {string.Join(",", args)}");

        var optionsBuilder = new DbContextOptionsBuilder<LeadManagerDbContext>();
        optionsBuilder.UseSqlServer("PASTE THE CONNECTION STRING HERE TO CREATE AND EXECUTE MIGRATIONS");

        return new LeadManagerDbContext(optionsBuilder.Options, default!);
    }
}
