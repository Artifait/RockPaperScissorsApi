namespace RockPaperScissorsApi.Models;

public class GameResult
{
    public int Id { get; set; } // Primary Key
    public string PlayerName { get; set; } = string.Empty;
    public string Outcome { get; set; } = string.Empty; // "win", "lose", "draw"
    public DateTime PlayedAt { get; set; } = DateTime.UtcNow;
}
