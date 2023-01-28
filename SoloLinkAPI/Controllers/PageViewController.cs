using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoloLinkAPI.Services;

namespace SoloLinkAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PageViewController : ControllerBase
{
    private readonly IPageViewService _pageViewService;

    public PageViewController(IPageViewService pageViewService)
    {
        _pageViewService = pageViewService;
    }

    [HttpPost("IncrementViewCount/{username}")]
    public Task<IActionResult> IncrementViewCount(string username)
    {
        return _pageViewService.IncrementViewCount(username);
    }

    [Authorize]
    [HttpGet("GetMyAnalytics")]
    public Task<IActionResult> GetAnalytics()
    {
        return _pageViewService.GetMyAnalytics();
    }
}