using System.Collections.Concurrent;
using TicTacToe.Server.Models;

namespace TicTacToe.Server.Services;

public interface IGameService
{
    Task<Game> CreateGameAsync(string playerId, string playerName, CharacterIcon characterIcon, bool isPrivate = false);
    Task<Game?> JoinGameAsync(string gameId, string playerId, string playerName, CharacterIcon characterIcon);
    Task<Game?> GetGameAsync(string gameId);
    Task<Game?> GetGameByPlayerAsync(string playerId);
    Task<bool> MakeMoveAsync(string gameId, string playerId, int row, int col);
    Task<bool> RemovePlayerAsync(string playerId);
    Task<List<Game>> GetWaitingGamesAsync();
    Task<bool> ResetGameAsync(string gameId);
}

public class GameService : IGameService
{
    private readonly ConcurrentDictionary<string, Game> _games = new();
    private readonly ConcurrentDictionary<string, string> _playerToGame = new();
    private readonly Random _random = new();

    /// <summary>
    /// Randomly selects the first player between Player1 and Player2 to ensure fairness
    /// </summary>
    /// <param name="game">The game instance</param>
    /// <returns>The randomly selected player to go first</returns>
    private Player? SelectRandomFirstPlayer(Game game)
    {
        if (game.Player1 == null || game.Player2 == null)
            return null;

        // Randomly choose between Player1 and Player2
        return _random.Next(2) == 0 ? game.Player1 : game.Player2;
    }

    public async Task<Game> CreateGameAsync(string playerId, string playerName, CharacterIcon characterIcon, bool isPrivate = false)
    {
        var game = new Game
        {
            Id = Guid.NewGuid().ToString(),
            Player1 = new Player
            {
                Id = playerId,
                ConnectionId = playerId,
                Name = playerName,
                CharacterIcon = characterIcon,
                IsReady = true
            },
            State = GameState.WaitingForPlayers,
            IsPrivate = isPrivate
        };

        _games[game.Id] = game;
        _playerToGame[playerId] = game.Id;

        return await Task.FromResult(game);
    }

    public async Task<Game?> JoinGameAsync(string gameId, string playerId, string playerName, CharacterIcon characterIcon)
    {
        if (!_games.TryGetValue(gameId, out var game))
            return null;

        if (game.IsGameFull())
            return null;

        if (game.State != GameState.WaitingForPlayers)
            return null;

        // Check if the character icon is already taken by player1
        if (game.Player1 != null && game.Player1.CharacterIcon == characterIcon)
            return null;

        game.Player2 = new Player
        {
            Id = playerId,
            ConnectionId = playerId,
            Name = playerName,
            CharacterIcon = characterIcon,
            IsReady = true
        };

        game.State = GameState.InProgress;
        game.CurrentPlayer = SelectRandomFirstPlayer(game); // Randomly select first player for fairness
        game.StartedAt = DateTime.UtcNow;

        _playerToGame[playerId] = gameId;

        return await Task.FromResult(game);
    }

    public async Task<Game?> GetGameAsync(string gameId)
    {
        _games.TryGetValue(gameId, out var game);
        return await Task.FromResult(game);
    }

    public async Task<Game?> GetGameByPlayerAsync(string playerId)
    {
        if (!_playerToGame.TryGetValue(playerId, out var gameId))
            return null;

        return await GetGameAsync(gameId);
    }

    public async Task<bool> MakeMoveAsync(string gameId, string playerId, int row, int col)
    {
        if (!_games.TryGetValue(gameId, out var game))
            return false;

        if (game.State != GameState.InProgress)
            return false;

        if (!game.IsPlayerTurn(playerId))
            return false;

        if (!game.IsValidMove(row, col))
            return false;

        var player = game.Player1?.Id == playerId ? game.Player1 : game.Player2;
        if (player == null)
            return false;

        // Use the new CharacterIcon-based MakeMove method
        game.MakeMove(row, col, player.CharacterIcon);

        // Check for winner
        var result = game.CheckWinner();
        if (result != GameResult.None)
        {
            game.Result = result;
            game.State = GameState.Finished;
            game.EndedAt = DateTime.UtcNow;
        }

        return await Task.FromResult(true);
    }

    public async Task<bool> RemovePlayerAsync(string playerId)
    {
        if (!_playerToGame.TryGetValue(playerId, out var gameId))
            return false;

        if (!_games.TryGetValue(gameId, out var game))
            return false;

        // Remove player from game
        if (game.Player1?.Id == playerId)
            game.Player1 = null;
        else if (game.Player2?.Id == playerId)
            game.Player2 = null;

        _playerToGame.TryRemove(playerId, out _);

        // If no players left, remove the game
        if (game.Player1 == null && game.Player2 == null)
        {
            _games.TryRemove(gameId, out _);
        }
        else if (game.State == GameState.InProgress)
        {
            // If game was in progress, mark it as finished
            game.State = GameState.Finished;
            game.EndedAt = DateTime.UtcNow;
        }

        return await Task.FromResult(true);
    }

    public async Task<List<Game>> GetWaitingGamesAsync()
    {
        var waitingGames = _games.Values
            .Where(g => g.State == GameState.WaitingForPlayers && !g.IsGameFull())
            .ToList();

        return await Task.FromResult(waitingGames);
    }

    public async Task<bool> ResetGameAsync(string gameId)
    {
        if (!_games.TryGetValue(gameId, out var game))
            return false;

        if (game.State != GameState.Finished)
            return false;

        // Reset game state
        game.Board = new CellState[3, 3];
        game.State = GameState.InProgress;
        game.Result = GameResult.None;
        game.CurrentPlayer = SelectRandomFirstPlayer(game); // Randomly select first player for fairness
        game.MoveCount = 0;
        game.StartedAt = DateTime.UtcNow;
        game.EndedAt = null;

        return await Task.FromResult(true);
    }
} 