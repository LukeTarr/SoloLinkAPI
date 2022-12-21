using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoloLinkAPI.DTOs;
using SoloLinkAPI.Services;

namespace SoloLinkAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private ILogger _logger;

    public AuthController(ILogger<AuthController> logger, IAuthService authService)
    {
        _logger = logger;
        _authService = authService;
    }

    [HttpPost("Register")]
    public Task<Dictionary<string, string>> Register(RegisterPostDto body)
    {
        return _authService.Register(body);
    }

    [HttpPost("Login")]
    public Task<Dictionary<string, string>> Login(LoginPostDto body)
    {
        return _authService.Login(body);
    }

    [HttpPut("ChangePassword")]
    [Authorize]
    public Task<Dictionary<string, string>> ChangePassword(PasswordDTO body)
    {
        return _authService.ChangePassword(body);
    }

    [HttpPut("ChangeUsername")]
    [Authorize]
    public Task<Dictionary<string, string>> ChangeUsername(UsernameDTO body)
    {
        return _authService.ChangeUsername(body);
    }
}