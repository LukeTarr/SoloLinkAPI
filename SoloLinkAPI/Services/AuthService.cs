using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SoloLinkAPI.DTOs;
using SoloLinkDataAccess;

namespace SoloLinkAPI.Services;

public class AuthService : IAuthService
{
    private readonly IHttpContextAccessor _accessor;
    private readonly IConfiguration _configuration;
    private readonly SoloLinkDatabaseContext _context;
    private readonly ILogger _logger;

    public AuthService(SoloLinkDatabaseContext context, ILogger<AuthService> logger, IConfiguration configuration,
        IHttpContextAccessor accessor)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
        _accessor = accessor;
    }

    public async Task<IActionResult> Register(RegisterPostDto dto)
    {
        var res = new Dictionary<string, string>();

        if (!dto.Password.Equals(dto.RepeatPassword))
        {
            res.Add("error", "Passwords do not match");
            return new OkObjectResult(res);
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(row => dto.Username.Equals(row.Username) || dto.Email.Equals(row.Email));

        if (user != null)
        {
            string? error;

            if (user.Email.Equals(dto.Email))
                error = "Email";
            else
                error = "Username";

            res.Add("error", $"{error} already exists");
            return new OkObjectResult(res);
        }


        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        await _context.Users.AddAsync(new User(dto.Email, dto.Username, passwordHash));
        await _context.SaveChangesAsync();

        res.Add("message", "success");
        return new OkObjectResult(res);
    }

    public async Task<IActionResult> Login(LoginPostDto dto)
    {
        var res = new Dictionary<string, string>();

        var user = await _context.Users
            .FirstOrDefaultAsync(row => dto.Email.Equals(row.Email));

        if (user == null)
        {
            res.Add("error", $"No user with email {dto.Email} found");
            return new OkObjectResult(res);
        }

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
        {
            res.Add("error", $"Wrong password for user {dto.Email}");
            return new OkObjectResult(res);
        }

        res.Add("token", GenerateToken(user));
        return new OkObjectResult(res);
    }

    public async Task<IActionResult> ChangeUsername(UsernameDTO dto)
    {
        var res = new Dictionary<string, string>();

        await using var transaction = await _context.Database.BeginTransactionAsync();

        User userResult;

        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(row =>
                _accessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value.Equals(row.UserId.ToString()));

            if (user == null)
            {
                res.Add("error", "No user with found.");
                return new OkObjectResult(res);
            }

            var lookup = await _context.Users.FirstOrDefaultAsync(row => dto.Username.Equals(row.Username));
            if (lookup != null)
            {
                res.Add("error", "Username already taken.");
                return new OkObjectResult(res);
            }

            user.Username = dto.Username;

            _context.Users.Update(user);
            userResult = user;
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            _logger.Log(LogLevel.Critical, "trying to edit user account that doesn't exist");
            await transaction.RollbackAsync();
            res.Add("error", "Couldn't edit user account.");
            return new OkObjectResult(res);
        }

        res.Add("token", GenerateToken(userResult));
        return new OkObjectResult(res);
    }

    public async Task<IActionResult> ChangePassword(PasswordDTO dto)
    {
        var res = new Dictionary<string, string>();

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(row =>
                _accessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value.Equals(row.UserId.ToString()));

            if (user == null)
            {
                res.Add("error", "No user with found.");
                return new OkObjectResult(res);
            }

            if (!dto.Password.Equals(dto.RepeatPassword))
            {
                res.Add("error", "Passwords do not match.");
                return new OkObjectResult(res);
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.Password))
            {
                res.Add("error", "Current password is incorrect.");
                return new OkObjectResult(res);
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.Password = passwordHash;

            _context.Users.Update(user);


            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            _logger.Log(LogLevel.Critical, "trying to edit user account that doesn't exist");
            await transaction.RollbackAsync();
            res.Add("error", "Couldn't edit user account.");
            return new OkObjectResult(res);
        }

        res.Add("message", "success");
        return new OkObjectResult(res);
    }

    private string GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Name, user.Username)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration.GetSection("JWT:Secret").Value));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }
}