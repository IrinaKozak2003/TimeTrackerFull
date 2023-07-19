using System.ComponentModel.DataAnnotations;

namespace TimeTrackerServer.Dtos;

public class LoginRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = null!;
    [Required, DataType(DataType.Password) ]
    public string Password { get; set; } = null!;
}