using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoloLinkAPI.DTOs;
using SoloLinkDataAccess;

namespace SoloLinkAPI.Services;

public class ProfileService : IProfileService
{
    private readonly SoloLinkDatabaseContext _context;
    private readonly ILogger _logger;
    private readonly IHttpContextAccessor _accessor;

    public ProfileService(SoloLinkDatabaseContext context, ILogger<ProfileService> logger, IHttpContextAccessor accessor)
    {
        _context = context;
        _logger = logger;
        _accessor = accessor;
    }

    public async Task<IActionResult> GetUserContent(string username)
    {
        var res = new Dictionary<string, string>();

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(row => row.Username == username);

            if (user is null)
            {
                res.Add("Error", "Username not found");
                return new OkObjectResult(res);
            }

            var categories = _context.Categories.Where(row => row.UserId.Equals(user.UserId)).ToList();

            var categoryDtos = new List<CategoryDTO>();
            var linkDtos = new List<LinkDTO>();

            foreach (var category in categories)
            {
                categoryDtos.Add(new CategoryDTO(category.CategoryId, category.UserId, category.Title));

                var links = _context.Links.Where(row => row.CategoryId == category.CategoryId);

                foreach (var link in links)
                    linkDtos.Add(new LinkDTO(link.LinkId, link.CategoryId, link.URL, link.Title));
            }

            var content = new UserContent(user.Username, categoryDtos, linkDtos);
            return new OkObjectResult(content);
        }
        catch
        {
            _logger.Log(LogLevel.Critical, "trying to get user content failed");
            await transaction.RollbackAsync();
            res.Add("Error", "Couldn't edit category.");
            return new OkObjectResult(res);
        }
    }

    public Task<IActionResult> GetMyContent()
    {
        return GetUserContent(_accessor.HttpContext.User.Identity.Name);
    }
    
    
}