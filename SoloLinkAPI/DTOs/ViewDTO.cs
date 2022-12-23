namespace SoloLinkAPI.DTOs;

public class ViewDTO
{
    public ViewDTO()
    {
    }

    public ViewDTO(int userId, DateTime viewDateTime)
    {
        UserId = userId;
        ViewDateTime = viewDateTime;
    }

    public int UserId { get; set; }
    public DateTime ViewDateTime { get; set; }
}