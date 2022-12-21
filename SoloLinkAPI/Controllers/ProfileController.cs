using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoloLinkAPI.Services;

namespace SoloLinkAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;
    private ILogger _logger;

    public ProfileController(ILogger<ProfileController> logger, IProfileService profileService)
    {
        _logger = logger;
        _profileService = profileService;
    }

    [HttpGet("GetContent/{username}")]
    public Task<IActionResult> GetUserContent(string username)
    {
        return _profileService.GetUserContent(username);
    }

    [Authorize]
    [HttpGet("GetMyContent")]
    public Task<IActionResult> GetMyContent()
    {
        return _profileService.GetMyContent();
    }
}