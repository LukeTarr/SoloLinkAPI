using Microsoft.AspNetCore.Mvc;

namespace SoloLinkAPI.Services;

public interface IPageViewService
{
    public Task<IActionResult> IncrementViewCount(string username);
    public Task<IActionResult> GetMyPageViews();
}