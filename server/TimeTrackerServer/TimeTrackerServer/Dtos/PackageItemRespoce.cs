namespace TimeTrackerServer.Dtos;

public class PackageItemResponse
{
    public string Id { get; set; }
        public string Status { get; set; }
    public string PackageName { get; set; }
    public decimal UsedBudgetInPresents { get; set; }
    public TimeSpan TotalTime { get; set; }
    
}