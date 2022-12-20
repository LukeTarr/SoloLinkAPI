using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SoloLinkAPI.DTOs;
using SoloLinkDataAccess;

namespace SoloLinkAPI.Services;

public class LinkService : ILinkService
{
    private readonly IHttpContextAccessor _accessor;
    private readonly SoloLinkDatabaseContext _context;
    private readonly ILogger _logger;

    public LinkService(IHttpContextAccessor accessor, SoloLinkDatabaseContext context,
        ILogger<LinkService> logger)
    {
        _accessor = accessor;
        _context = context;
        _logger = logger;
    }

    public async Task<Dictionary<string, string>> Post(LinkDTO dto)
    {
        return await DoTransactionalQuery("post", dto: dto);
    }

    public async Task<Dictionary<string, string>> Put(int id, LinkDTO dto)
    {
        return await DoTransactionalQuery("put", id, dto);
    }

    public async Task<Dictionary<string, string>> Delete(int id)
    {
        return await DoTransactionalQuery("delete", id);
    }

    private async Task<Dictionary<string, string>> DoTransactionalQuery(string action, int id = 0, LinkDTO dto = null)
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
            Link link = null;
            Category categoryCurrent = null;
            Category category = null;

            if (action != "delete")
                category = await _context.Categories.FirstOrDefaultAsync(row => row.CategoryId == dto.CategoryId);


            if (id != 0)
            {
                link = await _context.Links.FirstOrDefaultAsync(row => row.LinkId == id);

                if (link is null)
                {
                    res.Add("Error", "Link doesn't exist");
                    return res;
                }

                categoryCurrent =
                    await _context.Categories.FirstOrDefaultAsync(row => row.CategoryId == link.CategoryId);

                if (!categoryCurrent.UserId.Equals(int.Parse(_accessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value)))
                {
                    res.Add("Error", "Unauthorized to access that link");
                    return res;
                }

                if (action == "delete") category = categoryCurrent;
            }

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

            if (action.Equals("post"))
            {
                _context.Links.Add(new Link(dto.CategoryId, dto.URL, dto.Title));
            }
            else if (action.Equals("put"))
            {
                link.CategoryId = dto.CategoryId;
                link.Title = dto.Title ?? link.Title;
                link.URL = dto.URL ?? link.URL;

                _context.Links.Update(link);
            }
            else
            {
                _context.Remove(link);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            _logger.Log(LogLevel.Critical, "trying to edit Link that doesn't exist");
            await transaction.RollbackAsync();
            res.Add("Error", "Couldn't edit Link.");
            return res;
        }

        res.Add("Message", "Success");
        return res;
    }
}