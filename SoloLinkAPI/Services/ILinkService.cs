using SoloLinkAPI.DTOs;

namespace SoloLinkAPI.Services;

public interface ILinkService
{
    public Task<Dictionary<string, string>> Post(LinkDTO dto);
    public Task<Dictionary<string, string>> Put(int id, LinkDTO dto);
    public Task<Dictionary<string, string>> Delete(int id);
}