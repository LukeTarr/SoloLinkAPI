using Microsoft.AspNetCore.Mvc;
using SoloLinkAPI.DTOs;

namespace SoloLinkAPI.Services;

public interface IAuthService
{
    public Task<IActionResult> Register(RegisterPostDto dto);
    public Task<IActionResult> Login(LoginPostDto dto);
    public Task<IActionResult> ChangeUsername(UsernameDTO dto);
    public Task<IActionResult> ChangePassword(PasswordDTO dto);
}