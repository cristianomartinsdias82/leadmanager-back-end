﻿using Application.Contracts.Persistence;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Shared.Tests
{
    public static class InMemoryLeadManagerDbContextFactory
    {
        public static ILeadManagerDbContext Create(string? databaseName = default)
        {
            var options = new DbContextOptionsBuilder<LeadManagerDbContext>()
                            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
                            .Options;

            return new LeadManagerDbContext(options);
        }
    }
}