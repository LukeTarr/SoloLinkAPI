using System.ComponentModel.DataAnnotations;

namespace SoloLinkAPI.DTOs;

public class RegisterPostDto
{
    [Required] public string Username { get; set; }

    [Required] public string Email { get; set; }

    [Required] public string Password { get; set; }

    [Required] public string RepeatPassword { get; set; }
}