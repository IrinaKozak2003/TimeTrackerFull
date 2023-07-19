namespace TimeTrackerServer.Models;

public class TimeTrackerDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;


    public string PackageCollectionName { get; set; } = null!;
    
    public string CycleCollectionName { get; set; } = null!;
}