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

    private string CellStateToCharacterIcon(CellState cellState, Game game)
    {
        if (cellState == CellState.Empty)
        {
            return "";
        }
        
        // Find the player who has this cell state and return their character icon
        var player1CellState = game.Player1?.CharacterIcon switch
        {
            CharacterIcon.Cross => CellState.Cross,
            CharacterIcon.Circle => CellState.Circle,
            CharacterIcon.Kuromi => CellState.Kuromi,
            CharacterIcon.MyMelody => CellState.MyMelody,
            CharacterIcon.Spiderman => CellState.Spiderman,
            CharacterIcon.Cinnamoroll => CellState.Cinnamoroll,
            _ => CellState.Cross
        };
        
        var player2CellState = game.Player2?.CharacterIcon switch
        {
            CharacterIcon.Cross => CellState.Cross,
            CharacterIcon.Circle => CellState.Circle,
            CharacterIcon.Kuromi => CellState.Kuromi,
            CharacterIcon.MyMelody => CellState.MyMelody,
            CharacterIcon.Spiderman => CellState.Spiderman,
            CharacterIcon.Cinnamoroll => CellState.Cinnamoroll,
            _ => CellState.Circle
        };
        
        if (cellState == player1CellState)
        {
            return game.Player1?.CharacterIcon.ToString() ?? "";
        }
        else if (cellState == player2CellState)
        {
            return game.Player2?.CharacterIcon.ToString() ?? "";
        }
        else
        {
            return "";
        }
    }

    public async Task CreateGame(string playerName, string characterIcon, bool isPrivate = false)
    {
        try
        {
            var playerId = Context.ConnectionId;
            
            // Parse character icon
            if (!Enum.TryParse<CharacterIcon>(characterIcon, true, out var parsedIcon))
            {
                await Clients.Caller.SendAsync("Error", "Invalid character icon");
                return;
            }

            var game = await _gameService.CreateGameAsync(playerId, playerName, parsedIcon, isPrivate);
            
            // Join the game room
            await Groups.AddToGroupAsync(Context.ConnectionId, game.Id);
            
            // Notify the player about the created game
            await Clients.Caller.SendAsync("GameCreated", new
            {
                GameId = game.Id,
                PlayerId = playerId,
                PlayerName = playerName,
                CharacterIcon = game.Player1?.CharacterIcon.ToString(),
                State = game.State.ToString(),
                IsPrivate = game.IsPrivate
            });

            _logger.LogInformation($"Game {game.Id} created by player {playerName} with character {parsedIcon} (Private: {isPrivate})");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating game");
            await Clients.Caller.SendAsync("Error", "Failed to create game");
        }
    }

    public async Task JoinGame(string gameId, string playerName, string characterIcon)
    {
        try
        {
            var playerId = Context.ConnectionId;
            
            // Parse character icon
            if (!Enum.TryParse<CharacterIcon>(characterIcon, true, out var parsedIcon))
            {
                await Clients.Caller.SendAsync("Error", "Invalid character icon");
                return;
            }

            var game = await _gameService.JoinGameAsync(gameId, playerId, playerName, parsedIcon);
            
            if (game == null)
            {
                await Clients.Caller.SendAsync("Error", "Game not found, already full, or character already taken");
                return;
            }

            // Join the game room
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            
            // Notify both players about the game start
            await Clients.Group(gameId).SendAsync("GameStarted", new
            {
                GameId = gameId,
                Player1 = new { 
                    Id = game.Player1?.Id, 
                    Name = game.Player1?.Name, 
                    CharacterIcon = game.Player1?.CharacterIcon.ToString()
                },
                Player2 = new { 
                    Id = game.Player2?.Id, 
                    Name = game.Player2?.Name, 
                    CharacterIcon = game.Player2?.CharacterIcon.ToString()
                },
                CurrentPlayer = new { 
                    Id = game.CurrentPlayer?.Id, 
                    Name = game.CurrentPlayer?.Name, 
                    CharacterIcon = game.CurrentPlayer?.CharacterIcon.ToString()
                },
                State = game.State.ToString()
            });

            _logger.LogInformation($"Player {playerName} joined game {gameId} with character {parsedIcon}");
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

            // Create board representation for client
            var boardForClient = new string[3][];
            for (int i = 0; i < 3; i++)
            {
                boardForClient[i] = new string[3];
                for (int j = 0; j < 3; j++)
                {
                    boardForClient[i][j] = CellStateToCharacterIcon(game.Board[i, j], game);
                }
            }

            // Notify all players about the move
            await Clients.Group(gameId).SendAsync("MoveMade", new
            {
                GameId = gameId,
                Row = row,
                Col = col,
                PlayerId = playerId,
                Board = boardForClient,
                CurrentPlayer = game.CurrentPlayer != null ? new { 
                    Id = game.CurrentPlayer.Id, 
                    Name = game.CurrentPlayer.Name, 
                    CharacterIcon = game.CurrentPlayer.CharacterIcon.ToString()
                } : null,
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
            var gamesForClient = games.Select(g => new
            {
                GameId = g.Id,
                Player1Name = g.Player1?.Name,
                Player1CharacterIcon = g.Player1?.CharacterIcon.ToString(),
                CreatedAt = g.CreatedAt,
                IsPrivate = g.IsPrivate
            });

            await Clients.Caller.SendAsync("WaitingGames", gamesForClient);
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
                await Clients.Caller.SendAsync("Error", "Failed to reset game");
                return;
            }

            var game = await _gameService.GetGameAsync(gameId);
            if (game == null)
            {
                await Clients.Caller.SendAsync("Error", "Game not found");
                return;
            }

            // Create board representation for client
            var boardForClient = new string[3][];
            for (int i = 0; i < 3; i++)
            {
                boardForClient[i] = new string[3];
                for (int j = 0; j < 3; j++)
                {
                    boardForClient[i][j] = CellStateToCharacterIcon(game.Board[i, j], game);
                }
            }

            await Clients.Group(gameId).SendAsync("GameReset", new
            {
                GameId = gameId,
                Board = boardForClient,
                CurrentPlayer = new { 
                    Id = game.CurrentPlayer?.Id, 
                    Name = game.CurrentPlayer?.Name, 
                    CharacterIcon = game.CurrentPlayer?.CharacterIcon.ToString()
                },
                State = game.State.ToString()
            });
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
                var disconnectedPlayer = game.Player1?.Id == playerId ? game.Player1 : game.Player2;
                
                if (disconnectedPlayer != null)
                {
                    await Clients.Group(game.Id).SendAsync("PlayerDisconnected", new
                    {
                        GameId = game.Id,
                        PlayerId = playerId,
                        PlayerName = disconnectedPlayer.Name
                    });
                }
                
                await _gameService.RemovePlayerAsync(playerId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling disconnect");
        }
        
        await base.OnDisconnectedAsync(exception);
    }
} 