namespace TicTacToe.Server.Models;

public class Game
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public Player? Player1 { get; set; }
    public Player? Player2 { get; set; }
    public CellState[,] Board { get; set; } = new CellState[3, 3];
    public GameState State { get; set; } = GameState.WaitingForPlayers;
    public GameResult Result { get; set; } = GameResult.None;
    public Player? CurrentPlayer { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public int MoveCount { get; set; } = 0;
    public bool IsPrivate { get; set; } = false;

    public bool IsPlayerTurn(string playerId)
    {
        return CurrentPlayer?.Id == playerId;
    }

    public bool IsGameFull()
    {
        return Player1 != null && Player2 != null;
    }

    public bool IsValidMove(int row, int col)
    {
        return row >= 0 && row < 3 && col >= 0 && col < 3 && Board[row, col] == CellState.Empty;
    }

    private CellState CharacterIconToCellState(CharacterIcon icon)
    {
        return icon switch
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
    }

    public void MakeMove(int row, int col, CharacterIcon characterIcon)
    {
        if (IsValidMove(row, col))
        {
            Board[row, col] = CharacterIconToCellState(characterIcon);
            MoveCount++;
            
            // Switch turns
            CurrentPlayer = CurrentPlayer?.Id == Player1?.Id ? Player2 : Player1;
        }
    }

    // Keep the old method for backward compatibility


    private CellState GetPlayerCellState(Player player)
    {
        return CharacterIconToCellState(player.CharacterIcon);
    }

    public GameResult CheckWinner()
    {
        if (Player1 == null || Player2 == null) return GameResult.None;

        var player1CellState = GetPlayerCellState(Player1);
        var player2CellState = GetPlayerCellState(Player2);

        // Check rows
        for (int row = 0; row < 3; row++)
        {
            if (Board[row, 0] != CellState.Empty && Board[row, 0] == Board[row, 1] && Board[row, 1] == Board[row, 2])
            {
                return Board[row, 0] == player1CellState ? GameResult.Player1Wins : GameResult.Player2Wins;
            }
        }

        // Check columns
        for (int col = 0; col < 3; col++)
        {
            if (Board[0, col] != CellState.Empty && Board[0, col] == Board[1, col] && Board[1, col] == Board[2, col])
            {
                return Board[0, col] == player1CellState ? GameResult.Player1Wins : GameResult.Player2Wins;
            }
        }

        // Check diagonals
        if (Board[0, 0] != CellState.Empty && Board[0, 0] == Board[1, 1] && Board[1, 1] == Board[2, 2])
        {
            return Board[0, 0] == player1CellState ? GameResult.Player1Wins : GameResult.Player2Wins;
        }

        if (Board[0, 2] != CellState.Empty && Board[0, 2] == Board[1, 1] && Board[1, 1] == Board[2, 0])
        {
            return Board[0, 2] == player1CellState ? GameResult.Player1Wins : GameResult.Player2Wins;
        }

        // Check for draw
        if (MoveCount >= 9)
        {
            return GameResult.Draw;
        }

        return GameResult.None;
    }
} 