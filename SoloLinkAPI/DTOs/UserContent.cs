namespace SoloLinkAPI.DTOs;

public class UserContent
{
    public UserContent()
    {
    }

    public UserContent(string username, List<CategoryDTO> categoryDtos, List<LinkDTO> linkDtos)
    {
        Username = username;
        CategoryDtos = categoryDtos;
        LinkDtos = linkDtos;
    }

    public string Username { get; set; }

    public List<CategoryDTO> CategoryDtos { get; set; }

    public List<LinkDTO> LinkDtos { get; set; }
}