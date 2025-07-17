using Microsoft.EntityFrameworkCore;
using RockPaperScissorsApi.Models;

namespace RockPaperScissorsApi.Data;

public class AppDbContext : DbContext
{
    public DbSet<GameResult> GameResults => Set<GameResult>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.EnsureCreated(); 
    }
}
