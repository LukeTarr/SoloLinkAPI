using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoloLinkAPI.DTOs;
using SoloLinkAPI.Services;

namespace SoloLinkAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class LinkController : ControllerBase
{
    private readonly ILinkService _linkService;
    private ILogger _logger;

    public LinkController(ILinkService linkService, ILogger<LinkController> logger)
    {
        _linkService = linkService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize]
    public Task<IActionResult> Post(LinkDTO body)
    {
        return _linkService.Post(body);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public Task<IActionResult> Put(int id, LinkDTO body)
    {
        return _linkService.Put(id, body);
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public Task<IActionResult> Delete(int id)
    {
        return _linkService.Delete(id);
    }
}