using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoloLinkAPI.DTOs;
using SoloLinkDataAccess;

namespace SoloLinkAPI.Services;

public class CategoryService : ICategoryService
{
    private readonly IHttpContextAccessor _accessor;
    private readonly SoloLinkDatabaseContext _context;
    private readonly ILogger _logger;

    public CategoryService(IHttpContextAccessor accessor, SoloLinkDatabaseContext context,
        ILogger<CategoryService> logger)
    {
        _accessor = accessor;
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Post(CategoryDTO dto)
    {
        var res = new Dictionary<string, string>();

        _context.Categories.AddAsync(
            new Category(int.Parse(_accessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value), dto.Title));
        await _context.SaveChangesAsync();

        res.Add("message", "success");
        return new OkObjectResult(res);
    }

    public async Task<IActionResult> Put(int id, CategoryDTO dto)
    {
        return await DoTransactionalQuery(id, dto, false);
    }

    public async Task<IActionResult> Delete(int id)
    {
        return await DoTransactionalQuery(id, null, true);
    }

    private async Task<IActionResult> DoTransactionalQuery(int id, CategoryDTO? dto, bool isRemove)
    {
        var res = new Dictionary<string, string>();

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var category = await _context.Categories.FirstOrDefaultAsync(row => row.CategoryId == id);

            if (category is null)
            {
                res.Add("error", "category doesn't exist");
                return new OkObjectResult(res);
            }

            if (!category.UserId.Equals(
                    int.Parse(_accessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value)))
            {
                res.Add("error", "unauthorized to access that category");
                return new OkObjectResult(res);
            }

            if (isRemove)
            {
                _context.Remove(category);
            }
            else
            {
                category.Title = dto.Title;
                _context.Categories.Update(category);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            _logger.Log(LogLevel.Critical, "trying to edit Category that doesn't exist");
            await transaction.RollbackAsync();
            res.Add("error", "couldn't edit category.");
            return new OkObjectResult(res);
        }

        res.Add("message", "success");
        return new OkObjectResult(res);
    }
}