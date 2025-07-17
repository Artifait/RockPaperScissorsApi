namespace RockPaperScissorsApi.Models;

public class PlayerStats
{
    public string PlayerName { get; set; } = string.Empty;
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Draws { get; set; }

    public int TotalGames => Wins + Losses + Draws;
}
