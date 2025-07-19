using System.Collections.Concurrent;
using TicTacToe.Server.Models;
using TicTacToe.Server.Data;
using TicTacToe.Server.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace TicTacToe.Server.Services;

public interface IGameService
{
    Task<Game> CreateGameAsync(string playerId, string playerName, CharacterIcon characterIcon, bool isPrivate = false, bool playWithAI = false);
    Task<Game?> JoinGameAsync(string gameId, string playerId, string playerName, CharacterIcon characterIcon);
    Task<Game?> GetGameAsync(string gameId);
    Task<Game?> GetGameByPlayerAsync(string playerId);
    Task<bool> MakeMoveAsync(string gameId, string playerId, int row, int col);
    Task<(bool success, int row, int col)> MakeAIMoveAsync(string gameId);
    Task<bool> RemovePlayerAsync(string playerId);
    Task<List<Game>> GetWaitingGamesAsync();
    Task<bool> ResetGameAsync(string gameId);
}

public class GameService : IGameService
{
    private readonly ConcurrentDictionary<string, Game> _games = new();
    private readonly ConcurrentDictionary<string, string> _playerToGame = new();
    private readonly Random _random = new();
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public GameService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    /// <summary>
    /// Helper method to get the most recent game entity by gameId (ordered by Id descending)
    /// </summary>
    private async Task<GameEntity?> GetMostRecentGameEntityAsync(TicTacToeDbContext context, string gameId)
    {
        return await context.Games
            .Where(g => g.GameId == gameId)
            .OrderByDescending(g => g.Id)
            .FirstOrDefaultAsync();
    }

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

    /// <summary>
    /// Convert Game model to GameEntity for database operations
    /// </summary>
    private GameEntity ConvertToEntity(Game game)
    {
        return new GameEntity
        {
            GameId = game.Id,
            Player1Id = game.Player1?.Id ?? string.Empty,
            Player1Name = game.Player1?.Name ?? string.Empty,
            Player1Character = game.Player1?.CharacterIcon ?? CharacterIcon.Cross,
            Player2Id = game.Player2?.Id,
            Player2Name = game.Player2?.Name,
            Player2Character = game.Player2?.CharacterIcon,
            IsPrivate = game.IsPrivate,
            State = game.State,
            Result = game.Result,
            WinnerId = game.Result == GameResult.Player1Wins ? game.Player1?.Id :
                      game.Result == GameResult.Player2Wins ? game.Player2?.Id : null,
            WinnerName = game.Result == GameResult.Player1Wins ? game.Player1?.Name :
                        game.Result == GameResult.Player2Wins ? game.Player2?.Name : null,
            CreatedAt = game.CreatedAt,
            StartedAt = game.StartedAt,
            EndedAt = game.EndedAt,
            TotalMoves = game.MoveCount,
            Duration = game.StartedAt.HasValue && game.EndedAt.HasValue ?
                      game.EndedAt.Value - game.StartedAt.Value : null
        };
    }

    public async Task<Game> CreateGameAsync(string playerId, string playerName, CharacterIcon characterIcon, bool isPrivate = false, bool playWithAI = false)
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

        // If playing with AI, automatically add AI as player2 and start the game
        if (playWithAI)
        {
            game.Player2 = new Player
            {
                Id = "AI_PLAYER",
                ConnectionId = "AI_CONNECTION",
                Name = "AI",
                CharacterIcon = CharacterIcon.AI,
                IsReady = true
            };
            
            game.State = GameState.InProgress;
            game.CurrentPlayer = SelectRandomFirstPlayer(game); // Randomly select first player for fairness
            game.StartedAt = DateTime.UtcNow;
        }

        _games[game.Id] = game;
        _playerToGame[playerId] = game.Id;

        // Save to database
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TicTacToeDbContext>();
            var gameEntity = ConvertToEntity(game);
            context.Games.Add(gameEntity);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log error but don't fail the in-memory operation
            Console.WriteLine($"Failed to save game to database: {ex.Message}");
        }

        return game;
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

        // Update database with player 2 information
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TicTacToeDbContext>();
            var gameEntity = await GetMostRecentGameEntityAsync(context, gameId);
            if (gameEntity != null)
            {
                gameEntity.Player2Id = game.Player2.Id;
                gameEntity.Player2Name = game.Player2.Name;
                gameEntity.Player2Character = game.Player2.CharacterIcon;
                gameEntity.State = game.State;
                gameEntity.StartedAt = game.StartedAt;

                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            // Log error but don't fail the in-memory operation
            Console.WriteLine($"Failed to update game in database: {ex.Message}");
        }

        return game;
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

            // Update database with final game statistics
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<TicTacToeDbContext>();
                var gameEntity = await GetMostRecentGameEntityAsync(context, gameId);
                if (gameEntity != null)
                {
                    gameEntity.State = game.State;
                    gameEntity.Result = game.Result;
                    gameEntity.EndedAt = game.EndedAt;
                    gameEntity.TotalMoves = game.MoveCount;
                    gameEntity.WinnerId = game.Result == GameResult.Player1Wins ? game.Player1?.Id :
                                         game.Result == GameResult.Player2Wins ? game.Player2?.Id : null;
                    gameEntity.WinnerName = game.Result == GameResult.Player1Wins ? game.Player1?.Name :
                                           game.Result == GameResult.Player2Wins ? game.Player2?.Name : null;
                    gameEntity.Duration = game.StartedAt.HasValue && game.EndedAt.HasValue ? 
                                         game.EndedAt.Value - game.StartedAt.Value : null;

                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Log error but don't fail the in-memory operation
                Console.WriteLine($"Failed to update completed game in database: {ex.Message}");
            }
        }

        return true;
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

        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TicTacToeDbContext>();
            var gameEntity = await GetMostRecentGameEntityAsync(context, gameId);
            if (gameEntity != null)
            {
                gameEntity.State = GameState.Abandoned;
                gameEntity.EndedAt = DateTime.UtcNow;
                gameEntity.Duration = gameEntity.StartedAt.HasValue ?
                                     DateTime.UtcNow - gameEntity.StartedAt.Value : null;

                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            // Log error but don't fail the in-memory operation
            Console.WriteLine($"Failed to update abandoned game in database: {ex.Message}");
        }

        return true;
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

        // Insert new database record for the reset game (play again)
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TicTacToeDbContext>();
            var newGameEntity = ConvertToEntity(game);
            // Reset CreatedAt to current time for the new game record
            newGameEntity.CreatedAt = DateTime.UtcNow;
            
            context.Games.Add(newGameEntity);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log error but don't fail the in-memory operation
            Console.WriteLine($"Failed to insert reset game record to database: {ex.Message}");
        }

        return await Task.FromResult(true);
    }

    /// <summary>
    /// Makes an AI move for the current AI player
    /// </summary>
    public async Task<(bool success, int row, int col)> MakeAIMoveAsync(string gameId)
    {
        if (!_games.TryGetValue(gameId, out var game))
            return (false, -1, -1);

        if (game.State != GameState.InProgress)
            return (false, -1, -1);

        if (game.CurrentPlayer?.Id != "AI_PLAYER")
            return (false, -1, -1);

        // Simple AI logic: try to win, then block, then take center, then take corners, then take edges
        var (row, col) = GetBestAIMove(game);
        
        if (row == -1 || col == -1)
            return (false, -1, -1);

        // Make the move
        var success = await MakeMoveAsync(gameId, "AI_PLAYER", row, col);
        return (success, row, col);
    }

    /// <summary>
    /// Simple AI strategy for tic-tac-toe
    /// </summary>
    private (int row, int col) GetBestAIMove(Game game)
    {
        var aiCellState = GetPlayerCellState(game.Player2);
        var humanCellState = GetPlayerCellState(game.Player1);

        // 1. Try to win
        var winMove = FindWinningMove(game.Board, aiCellState);
        if (winMove.row != -1) return winMove;

        // 2. Block opponent from winning
        var blockMove = FindWinningMove(game.Board, humanCellState);
        if (blockMove.row != -1) return blockMove;

        // 3. Take center if available
        if (game.Board[1, 1] == CellState.Empty)
            return (1, 1);

        // 4. Take corners
        var corners = new[] { (0, 0), (0, 2), (2, 0), (2, 2) };
        foreach (var (r, c) in corners)
        {
            if (game.Board[r, c] == CellState.Empty)
                return (r, c);
        }

        // 5. Take edges
        var edges = new[] { (0, 1), (1, 0), (1, 2), (2, 1) };
        foreach (var (r, c) in edges)
        {
            if (game.Board[r, c] == CellState.Empty)
                return (r, c);
        }

        return (-1, -1); // No move available
    }

    /// <summary>
    /// Find a winning move for the given player
    /// </summary>
    private (int row, int col) FindWinningMove(CellState[,] board, CellState playerCellState)
    {
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                if (board[row, col] == CellState.Empty)
                {
                    // Try this move
                    board[row, col] = playerCellState;
                    
                    // Check if this creates a win
                    bool isWin = CheckWinCondition(board, playerCellState);
                    
                    // Undo the move
                    board[row, col] = CellState.Empty;
                    
                    if (isWin)
                        return (row, col);
                }
            }
        }
        return (-1, -1);
    }

    /// <summary>
    /// Check if the given player has won
    /// </summary>
    private bool CheckWinCondition(CellState[,] board, CellState playerCellState)
    {
        // Check rows
        for (int row = 0; row < 3; row++)
        {
            if (board[row, 0] == playerCellState && board[row, 1] == playerCellState && board[row, 2] == playerCellState)
                return true;
        }

        // Check columns
        for (int col = 0; col < 3; col++)
        {
            if (board[0, col] == playerCellState && board[1, col] == playerCellState && board[2, col] == playerCellState)
                return true;
        }

        // Check diagonals
        if (board[0, 0] == playerCellState && board[1, 1] == playerCellState && board[2, 2] == playerCellState)
            return true;

        if (board[0, 2] == playerCellState && board[1, 1] == playerCellState && board[2, 0] == playerCellState)
            return true;

        return false;
    }

    /// <summary>
    /// Get the cell state for a player
    /// </summary>
    private CellState GetPlayerCellState(Player? player)
    {
        if (player == null) return CellState.Empty;
        
        return player.CharacterIcon switch
        {
            CharacterIcon.Cross => CellState.Cross,
            CharacterIcon.Circle => CellState.Circle,
            CharacterIcon.Kuromi => CellState.Kuromi,
            CharacterIcon.MyMelody => CellState.MyMelody,
            CharacterIcon.Spiderman => CellState.Spiderman,
            CharacterIcon.Cinnamoroll => CellState.Cinnamoroll,
            CharacterIcon.BadtzMaru => CellState.BadtzMaru,
            CharacterIcon.HelloKitty => CellState.HelloKitty,
            CharacterIcon.Keroppi => CellState.Keroppi,
            CharacterIcon.Pochacco => CellState.Pochacco,
            CharacterIcon.AI => CellState.AI,
            _ => CellState.Empty
        };
    }
} 