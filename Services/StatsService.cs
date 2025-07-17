using Microsoft.EntityFrameworkCore;
using RockPaperScissorsApi.Data;
using RockPaperScissorsApi.Models;

namespace RockPaperScissorsApi.Services;

public class StatsService
{
    private readonly AppDbContext _db;

    public StatsService(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddResultAsync(GameResult result)
    {
        _db.GameResults.Add(result);
        await _db.SaveChangesAsync();
    }

    public async Task<PlayerStats?> GetStatsAsync(string playerName)
    {
        var results = await _db.GameResults
            .Where(r => r.PlayerName == playerName)
            .ToListAsync();

        if (!results.Any()) return null;

        return new PlayerStats
        {
            PlayerName = playerName,
            Wins = results.Count(r => r.Outcome == "win"),
            Losses = results.Count(r => r.Outcome == "lose"),
            Draws = results.Count(r => r.Outcome == "draw"),
        };
    }

    public async Task<List<PlayerStats>> GetLeaderboardAsync(int topN)
    {
        var grouped = await _db.GameResults
            .GroupBy(r => r.PlayerName)
            .Select(g => new PlayerStats
            {
                PlayerName = g.Key,
                Wins = g.Count(r => r.Outcome == "win"),
                Losses = g.Count(r => r.Outcome == "lose"),
                Draws = g.Count(r => r.Outcome == "draw")
            })
            .OrderByDescending(s => s.Wins)
            .ThenBy(s => s.Losses)
            .Take(topN)
            .ToListAsync();

        return grouped;
    }
}
