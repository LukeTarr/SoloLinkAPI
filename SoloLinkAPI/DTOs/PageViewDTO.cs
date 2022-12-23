namespace SoloLinkAPI.DTOs;

public class PageViewDTO
{
    public PageViewDTO()
    {
    }

    public PageViewDTO(string username, List<ViewDTO> views)
    {
        Username = username;
        Views = views;
    }

    public string Username { get; set; }

    public List<ViewDTO> Views { get; set; }
}