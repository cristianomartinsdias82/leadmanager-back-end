using Application.Contracts.Persistence;
using CrossCutting.Security.IAM;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Shared.Tests
{
    public static class InMemoryLeadManagerDbContextFactory
    {
        public static ILeadManagerDbContext Create(IUserService userUservice, string? databaseName = default)
        {
            var options = new DbContextOptionsBuilder<LeadManagerDbContext>()
                            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
                            .Options;

            return new LeadManagerDbContext(options, userUservice);
        }
    }
}