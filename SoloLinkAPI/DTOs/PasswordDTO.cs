using System.ComponentModel.DataAnnotations;

namespace SoloLinkAPI.DTOs;

public class PasswordDTO
{
    public PasswordDTO()
    {
    }

    public PasswordDTO(string password)
    {
        Password = password;
    }

    [Required] public string Password { get; set; }
    
}