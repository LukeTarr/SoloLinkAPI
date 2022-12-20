using SoloLinkAPI.DTOs;

namespace SoloLinkAPI.Services;

public interface IAuthService
{
    public Task<Dictionary<string, string>> Register(RegisterPostDto dto);
    public Task<Dictionary<string, string>> Login(LoginPostDto dto);
    public Task<Dictionary<string, string>> ChangeUsername(UsernameDTO dto);
    public Task<Dictionary<string, string>> ChangePassword(PasswordDTO dto);
}