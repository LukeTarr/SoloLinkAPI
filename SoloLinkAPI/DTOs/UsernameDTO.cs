using System.ComponentModel.DataAnnotations;

namespace SoloLinkAPI.DTOs;

public class UsernameDTO
{
    public UsernameDTO()
    {
    }

    public UsernameDTO(string username)
    {
        Username = username;
    }

    [Required] public string Username { get; set; }
}