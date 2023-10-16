using AspNetCore.Identity.MongoDbCore.Models;
using IAMServer.Persistence;
using MongoDbGenericRepository.Attributes;

namespace IAMServer.Entities;

[CollectionName(CollectionNames.Roles)]
public class ApplicationRole : MongoIdentityRole<Guid>
{
}