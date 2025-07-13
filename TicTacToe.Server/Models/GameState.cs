namespace TicTacToe.Server.Models;

public enum GameState
{
    WaitingForPlayers,
    InProgress,
    Finished
}

public enum CellState
{
    Empty,
    X,
    O
}

public enum GameResult
{
    None,
    Player1Wins,
    Player2Wins,
    Draw
} 