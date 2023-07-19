using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson;
using MongoDbGenericRepository.Attributes;
using Newtonsoft.Json;

namespace TimeTrackerServer.Models;

[CollectionName("users")]
public class ApplicationUser : MongoIdentityUser<Guid>
{
    
    public List<ObjectId> PackageIds { get; set; } = null!;
    public List<Cycle> Cycles { get; set; } = null!;
}
