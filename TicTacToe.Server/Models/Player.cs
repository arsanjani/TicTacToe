namespace TicTacToe.Server.Models;

public class Player
{
    public string Id { get; set; } = string.Empty;
    public string ConnectionId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public char Symbol { get; set; } // 'X' or 'O'
    public bool IsReady { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
} 