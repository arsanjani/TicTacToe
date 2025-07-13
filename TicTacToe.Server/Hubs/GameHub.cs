using Microsoft.AspNetCore.SignalR;
using TicTacToe.Server.Services;
using TicTacToe.Server.Models;

namespace TicTacToe.Server.Hubs;

public class GameHub : Hub
{
    private readonly IGameService _gameService;
    private readonly ILogger<GameHub> _logger;

    public GameHub(IGameService gameService, ILogger<GameHub> logger)
    {
        _gameService = gameService;
        _logger = logger;
    }

    public async Task CreateGame(string playerName, bool isPrivate = false)
    {
        try
        {
            var playerId = Context.ConnectionId;
            var game = await _gameService.CreateGameAsync(playerId, playerName, isPrivate);
            
            // Join the game room
            await Groups.AddToGroupAsync(Context.ConnectionId, game.Id);
            
            // Notify the player about the created game
            await Clients.Caller.SendAsync("GameCreated", new
            {
                GameId = game.Id,
                PlayerId = playerId,
                PlayerName = playerName,
                Symbol = game.Player1?.Symbol,
                State = game.State.ToString(),
                IsPrivate = game.IsPrivate
            });

            _logger.LogInformation($"Game {game.Id} created by player {playerName} (Private: {isPrivate})");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating game");
            await Clients.Caller.SendAsync("Error", "Failed to create game");
        }
    }

    public async Task JoinGame(string gameId, string playerName)
    {
        try
        {
            var playerId = Context.ConnectionId;
            var game = await _gameService.JoinGameAsync(gameId, playerId, playerName);
            
            if (game == null)
            {
                await Clients.Caller.SendAsync("Error", "Game not found or already full");
                return;
            }

            // Join the game room
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            
            // Notify both players about the game start
            await Clients.Group(gameId).SendAsync("GameStarted", new
            {
                GameId = gameId,
                Player1 = new { Id = game.Player1?.Id, Name = game.Player1?.Name, Symbol = game.Player1?.Symbol },
                Player2 = new { Id = game.Player2?.Id, Name = game.Player2?.Name, Symbol = game.Player2?.Symbol },
                CurrentPlayer = new { Id = game.CurrentPlayer?.Id, Name = game.CurrentPlayer?.Name, Symbol = game.CurrentPlayer?.Symbol },
                State = game.State.ToString()
            });

            _logger.LogInformation($"Player {playerName} joined game {gameId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining game");
            await Clients.Caller.SendAsync("Error", "Failed to join game");
        }
    }

    public async Task MakeMove(string gameId, int row, int col)
    {
        try
        {
            var playerId = Context.ConnectionId;
            var success = await _gameService.MakeMoveAsync(gameId, playerId, row, col);
            
            if (!success)
            {
                await Clients.Caller.SendAsync("Error", "Invalid move");
                return;
            }

            var game = await _gameService.GetGameAsync(gameId);
            if (game == null)
            {
                await Clients.Caller.SendAsync("Error", "Game not found");
                return;
            }

            // Notify all players about the move
            await Clients.Group(gameId).SendAsync("MoveMade", new
            {
                GameId = gameId,
                Row = row,
                Col = col,
                PlayerId = playerId,
                Board = ConvertBoardToArray(game.Board),
                CurrentPlayer = game.CurrentPlayer != null ? new { Id = game.CurrentPlayer.Id, Name = game.CurrentPlayer.Name, Symbol = game.CurrentPlayer.Symbol } : null,
                MoveCount = game.MoveCount
            });

            // Check if game is finished
            if (game.State == GameState.Finished)
            {
                await Clients.Group(gameId).SendAsync("GameFinished", new
                {
                    GameId = gameId,
                    Result = game.Result.ToString(),
                    Winner = game.Result == GameResult.Player1Wins ? game.Player1?.Name :
                             game.Result == GameResult.Player2Wins ? game.Player2?.Name : null,
                    EndedAt = game.EndedAt
                });
            }

            _logger.LogInformation($"Move made in game {gameId} at ({row}, {col}) by player {playerId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error making move");
            await Clients.Caller.SendAsync("Error", "Failed to make move");
        }
    }

    public async Task GetWaitingGames()
    {
        try
        {
            var games = await _gameService.GetWaitingGamesAsync();
            var gamesList = games.Select(g => new
            {
                GameId = g.Id,
                Player1Name = g.Player1?.Name,
                CreatedAt = g.CreatedAt,
                IsPrivate = g.IsPrivate
            }).ToList();

            await Clients.Caller.SendAsync("WaitingGames", gamesList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting waiting games");
            await Clients.Caller.SendAsync("Error", "Failed to get waiting games");
        }
    }

    public async Task ResetGame(string gameId)
    {
        try
        {
            var success = await _gameService.ResetGameAsync(gameId);
            
            if (!success)
            {
                await Clients.Caller.SendAsync("Error", "Cannot reset game");
                return;
            }

            var game = await _gameService.GetGameAsync(gameId);
            if (game == null)
            {
                await Clients.Caller.SendAsync("Error", "Game not found");
                return;
            }

            // Notify all players about the game reset
            await Clients.Group(gameId).SendAsync("GameReset", new
            {
                GameId = gameId,
                Board = ConvertBoardToArray(game.Board),
                CurrentPlayer = new { Id = game.CurrentPlayer?.Id, Name = game.CurrentPlayer?.Name, Symbol = game.CurrentPlayer?.Symbol },
                State = game.State.ToString()
            });

            _logger.LogInformation($"Game {gameId} reset");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting game");
            await Clients.Caller.SendAsync("Error", "Failed to reset game");
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            var playerId = Context.ConnectionId;
            var game = await _gameService.GetGameByPlayerAsync(playerId);
            
            if (game != null)
            {
                // Get the leaving player's name
                var leavingPlayer = game.Player1?.Id == playerId ? game.Player1 : game.Player2;
                var leavingPlayerName = leavingPlayer?.Name ?? "Unknown Player";

                // Notify other players about disconnection
                await Clients.OthersInGroup(game.Id).SendAsync("PlayerDisconnected", new
                {
                    GameId = game.Id,
                    PlayerId = playerId,
                    PlayerName = leavingPlayerName
                });

                // Remove player from game
                await _gameService.RemovePlayerAsync(playerId);
            }

            _logger.LogInformation($"Player {playerId} disconnected");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling disconnection");
        }

        await base.OnDisconnectedAsync(exception);
    }

    private static string[][] ConvertBoardToArray(CellState[,] board)
    {
        var result = new string[3][];
        for (int i = 0; i < 3; i++)
        {
            result[i] = new string[3];
            for (int j = 0; j < 3; j++)
            {
                result[i][j] = board[i, j] switch
                {
                    CellState.Empty => "",
                    CellState.X => "X",
                    CellState.O => "O",
                    _ => ""
                };
            }
        }
        return result;
    }
} 