using Newtonsoft.Json;

namespace TimeTrackerServer.Dtos;

public class BudgetRequest
{
    [JsonIgnore]
    public  string Id { get; set; } = null!;
     public string BudgetName { get; set; } = null!;
    
    public decimal Present  { get; set; }  
    
    public string UsedBudget { get; set; }
    public string Comment  { get; set; }=" ";
    public string UserId  { get; set; }= " ";
    public bool IsUser { get; set; } = false;

    
}