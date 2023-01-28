namespace SoloLinkAPI.DTOs;

public class ViewBucket
{
    public ViewBucket()
    {
    }

    public ViewBucket(DateTime date, int totalViews)
    {
        Date = date;
        TotalViews = totalViews;
    }

    public DateTime Date { get; set; }
    public int TotalViews { get; set; }
}