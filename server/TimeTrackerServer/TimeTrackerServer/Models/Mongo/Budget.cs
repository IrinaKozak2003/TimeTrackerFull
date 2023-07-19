using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TimeTrackerServer.Models;

public class Budget
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("Name")] public string BudgetName { get; set; } = null!;
    
    public decimal Present  { get; set; }  
    
    public TimeSpan UsedBudget { get; set; } 

    
}