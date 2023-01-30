using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoloLinkAPI.DTOs;
using SoloLinkAPI.Services;

namespace SoloLinkAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private ILogger _logger;

    public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize]
    public Task<IActionResult> Post(CategoryDTO body)
    {
        return _categoryService.Post(body);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public Task<IActionResult> Put(int id, CategoryDTO body)
    {
        return _categoryService.Put(id, body);
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public Task<IActionResult> Delete(int id)
    {
        return _categoryService.Delete(id);
    }
}