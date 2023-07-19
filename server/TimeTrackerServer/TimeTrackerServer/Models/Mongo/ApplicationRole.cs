using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace TimeTrackerServer.Models;
[CollectionName("roles")]
public class ApplicationRole: MongoIdentityRole<Guid>
{
    
}