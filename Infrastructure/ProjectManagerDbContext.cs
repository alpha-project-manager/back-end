using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure;

public class ProjectManagerDbContext : DbContext
{
    private readonly string _connectionString;

    // public DbSet<Project> Projects { get; init; }
    
    public ProjectManagerDbContext(IConfiguration configuration)
    {
        var readedConnString = configuration.GetConnectionString("DefaultConnection");
        if (readedConnString is null)
        {
            throw new Exception("Connection string \"DefaultConnection\" wasn't found in appsettings.json");
        }

        _connectionString = readedConnString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();
        optionsBuilder.UseNpgsql(_connectionString);
    }
}