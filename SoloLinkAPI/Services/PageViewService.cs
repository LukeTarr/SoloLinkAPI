using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoloLinkAPI.DTOs;
using SoloLinkDataAccess;

namespace SoloLinkAPI.Services;

public class PageViewService : IPageViewService
{
    private readonly IHttpContextAccessor _accessor;
    private readonly SoloLinkDatabaseContext _context;
    private readonly ILogger _logger;

    public PageViewService(IHttpContextAccessor accessor, SoloLinkDatabaseContext context,
        ILogger<IPageViewService> logger)
    {
        _accessor = accessor;
        _context = context;
        _logger = logger;
    }


    public async Task<IActionResult> IncrementViewCount(string username)
    {
        var res = new Dictionary<string, string>();

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(row => row.Username == username);

            if (user == null)
            {
                res.Add("Error", "Username not found");
                return new OkObjectResult(res);
            }

            await _context.PageViews.AddAsync(new PageView(user.UserId, DateTime.UtcNow));
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            res.Add("Message", "Success");
            return new OkObjectResult(res);
        }
        catch
        {
            _logger.Log(LogLevel.Critical, "Incrementing PageView count failed");
            await transaction.RollbackAsync();
            res.Add("Error", "Couldn't edit PageView.");
            return new OkObjectResult(res);
        }
    }

    public async Task<IActionResult> GetMyPageViews()
    {
        var res = new Dictionary<string, string>();

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(row =>
                row.UserId == int.Parse(_accessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value));

            if (user == null)
            {
                res.Add("Error", "Username not found");
                return new OkObjectResult(res);
            }


            var pageViews = _context.PageViews.Where(row => row.UserId.Equals(user.UserId)).ToList();
            var views = new List<ViewDTO>();
            foreach (var pageView in pageViews) views.Add(new ViewDTO(pageView.UserId, pageView.ViewDateTime));
            var pageViewDTO = new PageViewDTO(user.Username, views);

            return new OkObjectResult(pageViewDTO);
        }
        catch
        {
            _logger.Log(LogLevel.Critical, "Getting PageView count failed");
            await transaction.RollbackAsync();
            res.Add("Error", "Couldn't get PageView.");
            return new OkObjectResult(res);
        }
    }
}