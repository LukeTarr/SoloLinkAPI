using System.ComponentModel.DataAnnotations;

namespace SoloLinkAPI.DTOs;

public class LoginPostDto
{
    [Required] public string Email { get; set; }

    [Required] public string Password { get; set; }
}