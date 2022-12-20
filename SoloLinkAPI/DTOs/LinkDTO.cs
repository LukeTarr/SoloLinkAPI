namespace SoloLinkAPI.DTOs;

public class LinkDTO
{
    public LinkDTO()
    {
    }

    public LinkDTO(int linkId, int categoryId, string url, string title)
    {
        LinkId = linkId;
        CategoryId = categoryId;
        URL = url;
        Title = title;
    }

    public int LinkId { get; set; }

    public int CategoryId { get; set; }

    public string URL { get; set; }

    public string Title { get; set; }
}