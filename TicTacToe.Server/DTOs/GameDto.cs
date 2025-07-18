using TicTacToe.Server.Models;

namespace TicTacToe.Server.DTOs;

public class GameDto : BaseDto
{
    public string GameId { get; set; } = string.Empty;
    public string Player1Id { get; set; } = string.Empty;
    public string Player1Name { get; set; } = string.Empty;
    public CharacterIcon Player1Character { get; set; }
    public string? Player2Id { get; set; }
    public string? Player2Name { get; set; }
    public CharacterIcon? Player2Character { get; set; }
    public bool IsPrivate { get; set; }
    public GameState State { get; set; }
    public GameResult Result { get; set; }
    public string? WinnerId { get; set; }
    public string? WinnerName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public int TotalMoves { get; set; }
    public TimeSpan? Duration { get; set; }
} 