using Microsoft.AspNetCore.Mvc;

namespace SoloLinkAPI.Services;

public interface IProfileService
{
    public Task<IActionResult> GetUserContent(string username);
    public Task<IActionResult> GetMyContent();
    
}