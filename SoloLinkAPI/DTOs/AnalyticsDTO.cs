namespace SoloLinkAPI.DTOs;

public class AnalyticsDTO
{
    public AnalyticsDTO()
    {
    }

    public AnalyticsDTO(string username, List<ViewBucket> buckets)
    {
        Username = username;
        Buckets = buckets;
    }

    public string Username { get; set; }
    public List<ViewBucket> Buckets { get; set; }
}