using Microsoft.AspNetCore.Mvc;
using SoloLinkAPI.DTOs;

namespace SoloLinkAPI.Services;

public interface ICategoryService
{
    public Task<IActionResult> Post(CategoryDTO dto);
    public Task<IActionResult> Put(int id, CategoryDTO dto);
    public Task<IActionResult> Delete(int id);
}