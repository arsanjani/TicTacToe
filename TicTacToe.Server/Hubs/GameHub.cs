using Microsoft.AspNetCore.SignalR;
using TicTacToe.Server.Services;
using TicTacToe.Server.Models;

namespace TicTacToe.Server.Hubs;

public class GameHub : Hub
{
    private readonly IGameService _gameService;
    private readonly ILogger<GameHub> _logger;
    // Use a hub context so that we can safely broadcast to clients from background threads
    private readonly IHubContext<GameHub> _hubContext;

    public GameHub(IGameService gameService, ILogger<GameHub> logger, IHubContext<GameHub> hubContext)
    {
        _gameService = gameService;
        _logger = logger;
        _hubContext = hubContext;
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
            CharacterIcon.BadtzMaru => CellState.BadtzMaru,
            CharacterIcon.HelloKitty => CellState.HelloKitty,
            CharacterIcon.Keroppi => CellState.Keroppi,
            CharacterIcon.Pochacco => CellState.Pochacco,
            CharacterIcon.AI => CellState.AI,
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
            CharacterIcon.BadtzMaru => CellState.BadtzMaru,
            CharacterIcon.HelloKitty => CellState.HelloKitty,
            CharacterIcon.Keroppi => CellState.Keroppi,
            CharacterIcon.Pochacco => CellState.Pochacco,
            CharacterIcon.AI => CellState.AI,
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

    public async Task CreateGame(string playerName, string characterIcon, bool isPrivate = false, bool playWithAI = false)
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

            var game = await _gameService.CreateGameAsync(playerId, playerName, parsedIcon, isPrivate, playWithAI);
            
            // Join the game room
            await Groups.AddToGroupAsync(Context.ConnectionId, game.Id);

            if (playWithAI && game.State == GameState.InProgress)
            {
                // AI game started immediately, notify about game start
                await Clients.Caller.SendAsync("GameStarted", new
                {
                    GameId = game.Id,
                    Player1 = new
                    {
                        Id = game.Player1?.Id,
                        Name = game.Player1?.Name,
                        CharacterIcon = game.Player1?.CharacterIcon.ToString()
                    },
                    Player2 = new
                    {
                        Id = game.Player2?.Id,
                        Name = game.Player2?.Name,
                        CharacterIcon = game.Player2?.CharacterIcon.ToString()
                    },
                    CurrentPlayer = new
                    {
                        Id = game.CurrentPlayer?.Id,
                        Name = game.CurrentPlayer?.Name,
                        CharacterIcon = game.CurrentPlayer?.CharacterIcon.ToString()
                    },
                    State = game.State.ToString()
                });

                _logger.LogInformation($"AI game {game.Id} started immediately with player {playerName}");

                // If AI was selected to go first, make the first AI move after a short delay
                if (game.CurrentPlayer?.Id == "AI_PLAYER")
                {

                    await Task.Delay(1500); // 1.5 second delay for first AI move

                    var (aiSuccess, aiRow, aiCol) = await _gameService.MakeAIMoveAsync(game.Id);
                    if (aiSuccess)
                    {
                        var updatedGame = await _gameService.GetGameAsync(game.Id);
                        if (updatedGame != null)
                        {
                            // Create updated board representation
                            var updatedBoardForClient = new string[3][];
                            for (int i = 0; i < 3; i++)
                            {
                                updatedBoardForClient[i] = new string[3];
                                for (int j = 0; j < 3; j++)
                                {
                                    updatedBoardForClient[i][j] = CellStateToCharacterIcon(updatedGame.Board[i, j], updatedGame);
                                }
                            }

                            // Notify about AI's first move
                            await _hubContext.Clients.Group(game.Id).SendAsync("MoveMade", new
                            {
                                GameId = game.Id,
                                Row = aiRow,
                                Col = aiCol,
                                PlayerId = "AI_PLAYER",
                                Board = updatedBoardForClient,
                                CurrentPlayer = updatedGame.CurrentPlayer != null ? new
                                {
                                    Id = updatedGame.CurrentPlayer.Id,
                                    Name = updatedGame.CurrentPlayer.Name,
                                    CharacterIcon = updatedGame.CurrentPlayer.CharacterIcon.ToString()
                                } : null,
                                MoveCount = updatedGame.MoveCount
                            });
                        }
                    }
                }
            }
            else
            {
                // Normal game creation, waiting for players
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

            // If it's now AI's turn and game is still in progress, make AI move after a short delay
            if (game.State == GameState.InProgress && game.CurrentPlayer?.Id == "AI_PLAYER")
            {

                await Task.Delay(1000); // 1 second delay to make AI move visible

                var (aiSuccess, aiRow, aiCol) = await _gameService.MakeAIMoveAsync(gameId);
                if (aiSuccess)
                {
                    var updatedGame = await _gameService.GetGameAsync(gameId);
                    if (updatedGame != null)
                    {
                        // Create updated board representation
                        var updatedBoardForClient = new string[3][];
                        for (int i = 0; i < 3; i++)
                        {
                            updatedBoardForClient[i] = new string[3];
                            for (int j = 0; j < 3; j++)
                            {
                                updatedBoardForClient[i][j] = CellStateToCharacterIcon(updatedGame.Board[i, j], updatedGame);
                            }
                        }

                        // Notify about AI move
                        await _hubContext.Clients.Group(gameId).SendAsync("MoveMade", new
                        {
                            GameId = gameId,
                            Row = aiRow,
                            Col = aiCol,
                            PlayerId = "AI_PLAYER",
                            Board = updatedBoardForClient,
                            CurrentPlayer = updatedGame.CurrentPlayer != null ? new
                            {
                                Id = updatedGame.CurrentPlayer.Id,
                                Name = updatedGame.CurrentPlayer.Name,
                                CharacterIcon = updatedGame.CurrentPlayer.CharacterIcon.ToString()
                            } : null,
                            MoveCount = updatedGame.MoveCount
                        });

                        // Check if AI move finished the game
                        if (updatedGame.State == GameState.Finished)
                        {
                            await _hubContext.Clients.Group(gameId).SendAsync("GameFinished", new
                            {
                                GameId = gameId,
                                Result = updatedGame.Result.ToString(),
                                Winner = updatedGame.Result == GameResult.Player1Wins ? updatedGame.Player1?.Name :
                                        updatedGame.Result == GameResult.Player2Wins ? updatedGame.Player2?.Name : null,
                                EndedAt = updatedGame.EndedAt
                            });
                        }
                    }
                }

            }

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

            // If the AI is set to make the first move after a reset, trigger it after a short delay
            if (game.State == GameState.InProgress && game.CurrentPlayer?.Id == "AI_PLAYER")
            {
                // small delay so the UI shows the reset state first
                await Task.Delay(1000);

                var (aiSuccess, aiRow, aiCol) = await _gameService.MakeAIMoveAsync(gameId);
                if (aiSuccess)
                {
                    var updatedGame = await _gameService.GetGameAsync(gameId);
                    if (updatedGame != null)
                    {
                        // Prepare board for client
                        var updatedBoard = new string[3][];
                        for (int i = 0; i < 3; i++)
                        {
                            updatedBoard[i] = new string[3];
                            for (int j = 0; j < 3; j++)
                            {
                                updatedBoard[i][j] = CellStateToCharacterIcon(updatedGame.Board[i, j], updatedGame);
                            }
                        }

                        await _hubContext.Clients.Group(gameId).SendAsync("MoveMade", new
                        {
                            GameId = gameId,
                            Row = aiRow,
                            Col = aiCol,
                            PlayerId = "AI_PLAYER",
                            Board = updatedBoard,
                            CurrentPlayer = updatedGame.CurrentPlayer != null ? new
                            {
                                Id = updatedGame.CurrentPlayer.Id,
                                Name = updatedGame.CurrentPlayer.Name,
                                CharacterIcon = updatedGame.CurrentPlayer.CharacterIcon.ToString()
                            } : null,
                            MoveCount = updatedGame.MoveCount
                        });

                        // If the AI's move finished the game after reset, notify
                        if (updatedGame.State == GameState.Finished)
                        {
                            await _hubContext.Clients.Group(gameId).SendAsync("GameFinished", new
                            {
                                GameId = gameId,
                                Result = updatedGame.Result.ToString(),
                                Winner = updatedGame.Result == GameResult.Player1Wins ? updatedGame.Player1?.Name :
                                         updatedGame.Result == GameResult.Player2Wins ? updatedGame.Player2?.Name : null,
                                EndedAt = updatedGame.EndedAt
                            });
                        }
                    }
                }
            }
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