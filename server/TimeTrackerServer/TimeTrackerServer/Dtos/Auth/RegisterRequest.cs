using System.ComponentModel.DataAnnotations;
using System;
namespace TimeTrackerServer.Dtos;

public class RegisterRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = null!;
    [Required]
    public string Username { get; set; } = null!;
    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = null!;
    [Required, DataType(DataType.Password), Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = null!;

    public string Role { set; get; } = null!;

}