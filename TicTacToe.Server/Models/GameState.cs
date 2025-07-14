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
    Cross,           // icons8-cross-100.png
    Circle,          // icons8-circle-100-4.png
    Kuromi,          // icons8-kuromi-100.png
    MyMelody,        // icons8-my-melody-100.png
    Spiderman,       // icons8-spiderman-100-2.png
    Cinnamoroll      // icons8-cinnamoroll-100.png
}

public enum GameResult
{
    None,
    Player1Wins,
    Player2Wins,
    Draw
} 