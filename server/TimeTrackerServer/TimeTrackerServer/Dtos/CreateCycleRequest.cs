namespace TimeTrackerServer.Dtos;

public class CreateCycleRequest
{
    
    public string? UserId { get; set; }
    public string CycleTime { get; set; }
    public string? PackageId { get; set; } 
    public string BudgetId { get; set; }
    public string Comment { get; set; } = null!;
}