using System.ComponentModel.DataAnnotations;

namespace SoloLinkDataAccess;

public class PageView
{
    public PageView()
    {
    }

    public PageView(int userId, DateTime viewDateTime)
    {
        UserId = userId;
        ViewDateTime = viewDateTime;
    }

    [Key] public int PageViewId { get; set; }

    public User User { get; set; }

    public int UserId { get; set; }

    public DateTime ViewDateTime { get; set; }
}