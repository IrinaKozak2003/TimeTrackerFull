using TimeTrackerServer.Models;

namespace TimeTrackerServer.Dtos;

public class CreatePackageRequest
{ 
    public string? Id { get; set; }
    public string PackageName { get; set; } = null!;
    public string PackageBudget { get; set; }
    public string PackageDescription { get; set; } = null!;
    public string Status { get; set; } = null!;
    public List<User>? Users { get; set; } = null!;

}