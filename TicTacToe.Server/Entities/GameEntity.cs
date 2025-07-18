using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TicTacToe.Server.Models;

namespace TicTacToe.Server.Entities;

[Table("Games")]
public class GameEntity : BaseEntity
{
    [Required]
    [StringLength(50)]
    public string GameId { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Player1Id { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Player1Name { get; set; } = string.Empty;

    [Required]
    public CharacterIcon Player1Character { get; set; }

    [StringLength(100)]
    public string? Player2Id { get; set; }

    [StringLength(100)]
    public string? Player2Name { get; set; }

    public CharacterIcon? Player2Character { get; set; }

    [Required]
    public bool IsPrivate { get; set; } = false;

    [Required]
    public GameState State { get; set; } = GameState.WaitingForPlayers;

    [Required]
    public GameResult Result { get; set; } = GameResult.None;

    [StringLength(100)]
    public string? WinnerId { get; set; }

    [StringLength(100)]
    public string? WinnerName { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? StartedAt { get; set; }

    public DateTime? EndedAt { get; set; }

    [Required]
    public int TotalMoves { get; set; } = 0;

    [Required]
    public TimeSpan? Duration { get; set; }
} 