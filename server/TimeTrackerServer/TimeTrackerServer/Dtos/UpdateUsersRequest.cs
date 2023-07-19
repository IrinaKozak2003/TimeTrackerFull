namespace TimeTrackerServer.Dtos;

public class UpdateUsersRequest
{
    public List<string> UserIds { get; set; } = null!;
}