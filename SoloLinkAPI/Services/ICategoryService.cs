using SoloLinkAPI.DTOs;

namespace SoloLinkAPI.Services;

public interface ICategoryService
{
    public Task<Dictionary<string, string>> Post(CategoryDTO dto);
    public Task<Dictionary<string, string>> Put(int id, CategoryDTO dto);
    public Task<Dictionary<string, string>> Delete(int id);
}