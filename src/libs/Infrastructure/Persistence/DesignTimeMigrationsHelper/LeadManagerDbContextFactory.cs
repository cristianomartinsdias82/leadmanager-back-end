using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence.DesignTimeMigrationsHelper;

//https://go.microsoft.com/fwlink/?linkid=851728
//https://learn.microsoft.com/en-us/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli
public sealed class LeadManagerDbContextFactory : IDesignTimeDbContextFactory<LeadManagerDbContext>
{
    public LeadManagerDbContext CreateDbContext(string[] args)
    {
		var config = new ConfigurationBuilder()
						//.SetBasePath(Directory.GetCurrentDirectory())
						.SetBasePath(@"F:\Codebase\_.NET Solutions\Core\7.x\LeadManager\backend\src\LeadManager\src\hosts\LeadManagerApi")
						.AddJsonFile("appsettings.json")
						.Build();

        var optionsBuilder = new DbContextOptionsBuilder<LeadManagerDbContext>();

		optionsBuilder.UseSqlServer(config["DataSourceSettings:ConnectionString"]);

        return new(optionsBuilder.Options, default!, TimeProvider.System);
    }
}
