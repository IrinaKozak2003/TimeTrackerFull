namespace TimeTrackerServer.Dtos;

public class LoginResponse
{
    public bool Success { get; set; }
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string Message { get; set; } = null!;
}