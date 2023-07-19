using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TimeTrackerServer.Models;

public class Cycle
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string? UserId { get; set; }
    public TimeSpan CycleTime { get; set; }
    public string? PackageId { get; set; } 
    public string BudgetId { get; set; }
    public string Comment { get; set; } = null!;

}