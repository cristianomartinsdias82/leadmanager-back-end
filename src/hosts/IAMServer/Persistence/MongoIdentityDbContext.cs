using IAMServer.Entities;
using MongoDB.Driver;
using MongoDbGenericRepository;

namespace IAMServer.Persistence;

public class MongoIdentityDbContext : MongoDbContext, IMongoIdentityDbContext
{
    public MongoIdentityDbContext(string connectionString, string databaseName) : base(connectionString, databaseName) { }

    public MongoIdentityDbContext(string connectionString) : base(connectionString) { }

    public MongoIdentityDbContext(IMongoDatabase mongoDatabase) : base(mongoDatabase) { }

    public MongoIdentityDbContext(MongoClient client, string databaseName) : base(client, databaseName) { }

    private IMongoCollection<ApplicationUser> UsersCollection
        => Database.GetCollection<ApplicationUser>(CollectionNames.Users);

    public async Task<IEnumerable<ApplicationUser>> GetUsersAsync(CancellationToken cancellationToken = default)
        => await UsersCollection
                    .Find(FilterDefinition<ApplicationUser>.Empty)
                    .ToListAsync(cancellationToken);
}
