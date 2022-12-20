using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace SoloLinkDataAccess;

public class SoloLinkDatabaseContext : DbContext
{
    private readonly IConfiguration _configuration;

    public SoloLinkDatabaseContext(DbContextOptions<SoloLinkDatabaseContext> options, IConfiguration configuration) :
        base(options)
    {
        _configuration = configuration;
    }

    public DbSet<User>? Users { get; set; }

    public DbSet<Category>? Categories { get; set; }

    public DbSet<Link>? Links { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("WebApiDatabase"));
    }
}