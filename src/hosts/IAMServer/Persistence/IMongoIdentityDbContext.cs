using IAMServer.Entities;
using MongoDbGenericRepository;

namespace IAMServer.Persistence;

public interface IMongoIdentityDbContext : IMongoDbContext
{
    Task<IEnumerable<ApplicationUser>> GetUsersAsync(CancellationToken cancellationToken = default);
}
