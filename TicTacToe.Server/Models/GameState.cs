namespace TicTacToe.Server.Models;

public enum GameState
{
    WaitingForPlayers,
    InProgress,
    Finished,
    Abandoned
}

public enum CellState
{
    Empty,
    Cross,           // icons8-cross-100.png
    Circle,          // icons8-circle-100-4.png
    Kuromi,          // icons8-kuromi-100.png
    MyMelody,        // icons8-my-melody-100.png
    Spiderman,       // icons8-spiderman-100-2.png
    Cinnamoroll,     // icons8-cinnamoroll-100.png
    BadtzMaru,       // icons8-badtz-maru-100.png
    HelloKitty,      // icons8-hello-kitty-100.png
    Keroppi,         // icons8-keroppi-100.png
    Pochacco,        // icons8-pochacco-100.png
    AI               // icons8-ai-100.png
}

public enum GameResult
{
    None,
    Player1Wins,
    Player2Wins,
    Draw
} 