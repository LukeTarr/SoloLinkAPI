namespace SoloLinkAPI.DTOs;

public class CategoryDTO
{
    public CategoryDTO()
    {
    }

    public CategoryDTO(int categoryId, int userId, string title)
    {
        CategoryId = categoryId;
        UserId = userId;
        Title = title;
    }

    public int CategoryId { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; }
}