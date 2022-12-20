using System.ComponentModel.DataAnnotations;

namespace SoloLinkDataAccess;

public class Link
{
    public Link()
    {
    }

    public Link(int categoryId, string url, string title)
    {
        CategoryId = categoryId;
        URL = url;
        Title = title;
    }

    [Key] public int LinkId { get; set; }

    public Category Category { get; set; }

    public int CategoryId { get; set; }

    public string URL { get; set; }

    public string Title { get; set; }
}