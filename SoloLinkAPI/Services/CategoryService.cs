using System.Security.Claims;
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

    public async Task<Dictionary<string, string>> Post(CategoryDTO dto)
    {
        var res = new Dictionary<string, string>();

        if (_accessor.HttpContext == null)
        {
            res.Add("Error", "No Context");
            return res;
        }

        _context.Categories.AddAsync(new Category(int.Parse(_accessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value), dto.Title));
        await _context.SaveChangesAsync();

        res.Add("Message", "Success");
        return res;
    }

    public async Task<Dictionary<string, string>> Put(int id, CategoryDTO dto)
    {
        return await DoTransactionalQuery(id, dto, false);
    }

    public async Task<Dictionary<string, string>> Delete(int id)
    {
        return await DoTransactionalQuery(id, null, true);
    }

    private async Task<Dictionary<string, string>> DoTransactionalQuery(int id, CategoryDTO? dto, bool isRemove)
    {
        var res = new Dictionary<string, string>();

        if (_accessor.HttpContext == null)
        {
            res.Add("Error", "No Context");
            return res;
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var category = await _context.Categories.FirstOrDefaultAsync(row => row.CategoryId == id);

            if (category is null)
            {
                res.Add("Error", "Category doesn't exist");
                return res;
            }

            if (!category.UserId.Equals(int.Parse(_accessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value)))
            {
                res.Add("Error", "Unauthorized to access that category");
                return res;
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
            res.Add("Error", "Couldn't edit category.");
            return res;
        }

        res.Add("Message", "Success");
        return res;
    }
}