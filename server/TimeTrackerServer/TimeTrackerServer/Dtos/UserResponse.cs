using TimeTrackerServer.Models;

namespace TimeTrackerServer.Dtos;

public class UserResponse
{
    public List<User> Users { set; get; } = null!;
    public string Message { get; set; } = null!;
    public bool Success { get; set; }
    
}