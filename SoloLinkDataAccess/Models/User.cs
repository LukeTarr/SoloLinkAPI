using System.ComponentModel.DataAnnotations;

namespace SoloLinkDataAccess;

public class User
{
    public User()
    {
    }

    public User(string email, string username, string password)
    {
        Email = email;
        Username = username;
        Password = password;
    }

    [Key] public int UserId { get; set; }

    public string Email { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }
}