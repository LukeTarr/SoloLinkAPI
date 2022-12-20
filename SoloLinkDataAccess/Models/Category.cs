using System.ComponentModel.DataAnnotations;

namespace SoloLinkDataAccess;

public class Category
{
    public Category()
    {
    }

    public Category(int userId, string title)
    {
        UserId = userId;
        Title = title;
    }

    [Key] public int CategoryId { get; set; }

    public User User { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; }
}