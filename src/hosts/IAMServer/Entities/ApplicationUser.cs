using AspNetCore.Identity.MongoDbCore.Models;
using IAMServer.Persistence;
using MongoDbGenericRepository.Attributes;

namespace IAMServer.Entities;

[CollectionName(CollectionNames.Users)]
public class ApplicationUser : MongoIdentityUser<Guid>
{
}
