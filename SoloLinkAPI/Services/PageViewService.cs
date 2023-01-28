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
                res.Add("error", "Username not found");
                return new OkObjectResult(res);
            }

            await _context.PageViews.AddAsync(new PageView(user.UserId, DateTime.UtcNow));
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            res.Add("message", "success");
            return new OkObjectResult(res);
        }
        catch
        {
            _logger.Log(LogLevel.Critical, "Incrementing PageView count failed");
            await transaction.RollbackAsync();
            res.Add("error", "couldn't edit PageView.");
            return new OkObjectResult(res);
        }
    }

    public async Task<IActionResult> GetMyAnalytics()
    {
        var res = new Dictionary<string, string>();

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(row =>
                row.UserId == int.Parse(_accessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value));

            if (user == null)
            {
                res.Add("error", "user not found");
                return new OkObjectResult(res);
            }

            var viewList = _context.PageViews.Where(view => view.UserId == user.UserId)
                .GroupBy(view => view.ViewDateTime.Date)
                .Select(viewGroup => new ViewBucket { Date = viewGroup.Key, TotalViews = viewGroup.Sum(x => 1) })
                .ToList();


            var analytics = new AnalyticsDTO(user.Username, viewList);

            return new OkObjectResult(analytics);
        }
        catch
        {
            _logger.Log(LogLevel.Critical, "Getting Analytics failed");
            await transaction.RollbackAsync();
            res.Add("error", "couldn't get Analytics.");
            return new OkObjectResult(res);
        }
    }
}