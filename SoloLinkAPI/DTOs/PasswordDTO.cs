using System.ComponentModel.DataAnnotations;

namespace SoloLinkAPI.DTOs;

public class PasswordDTO
{
    public PasswordDTO()
    {
    }

    public PasswordDTO(string password, string currentPassword, string repeatPassword)
    {
        Password = password;
        CurrentPassword = currentPassword;
        RepeatPassword = repeatPassword;
    }

    [Required] public string Password { get; set; }

    [Required] public string CurrentPassword { get; set; }

    [Required] public string RepeatPassword { get; set; }
}