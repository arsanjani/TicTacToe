namespace TicTacToe.Server.Models;

public class Game
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public Player? Player1 { get; set; }
    public Player? Player2 { get; set; }
    public int BoardSize { get; set; } = 3;
    public CellState[,] Board { get; set; }
    public GameState State { get; set; } = GameState.WaitingForPlayers;
    public GameResult Result { get; set; } = GameResult.None;
    public Player? CurrentPlayer { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public int MoveCount { get; set; } = 0;
    public bool IsPrivate { get; set; } = false;

    public Game(int boardSize = 3)
    {
        BoardSize = boardSize;
        Board = new CellState[boardSize, boardSize];
    }

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
        return row >= 0 && row < BoardSize && col >= 0 && col < BoardSize && Board[row, col] == CellState.Empty;
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

    private CellState GetPlayerCellState(Player player)
    {
        return CharacterIconToCellState(player.CharacterIcon);
    }

    public GameResult CheckWinner()
    {
        if (Player1 == null || Player2 == null) return GameResult.None;

        var player1CellState = GetPlayerCellState(Player1);
        var player2CellState = GetPlayerCellState(Player2);
        var winLength = BoardSize; // For dynamic win condition: 3 in a row for 3x3, 4 in a row for 4x4

        // Check rows
        for (int row = 0; row < BoardSize; row++)
        {
            if (CheckLine(row, 0, 0, 1, winLength, player1CellState, player2CellState) != GameResult.None)
                return CheckLine(row, 0, 0, 1, winLength, player1CellState, player2CellState);
        }

        // Check columns
        for (int col = 0; col < BoardSize; col++)
        {
            if (CheckLine(0, col, 1, 0, winLength, player1CellState, player2CellState) != GameResult.None)
                return CheckLine(0, col, 1, 0, winLength, player1CellState, player2CellState);
        }

        // Check main diagonal (top-left to bottom-right)
        if (CheckLine(0, 0, 1, 1, winLength, player1CellState, player2CellState) != GameResult.None)
            return CheckLine(0, 0, 1, 1, winLength, player1CellState, player2CellState);

        // Check anti-diagonal (top-right to bottom-left)
        if (CheckLine(0, BoardSize - 1, 1, -1, winLength, player1CellState, player2CellState) != GameResult.None)
            return CheckLine(0, BoardSize - 1, 1, -1, winLength, player1CellState, player2CellState);

        // Check for draw (all cells filled)
        if (MoveCount >= BoardSize * BoardSize)
        {
            return GameResult.Draw;
        }

        return GameResult.None;
    }

    private GameResult CheckLine(int startRow, int startCol, int deltaRow, int deltaCol, int winLength, CellState player1CellState, CellState player2CellState)
    {
        CellState firstCell = Board[startRow, startCol];
        if (firstCell == CellState.Empty) return GameResult.None;

        for (int i = 1; i < winLength; i++)
        {
            int row = startRow + i * deltaRow;
            int col = startCol + i * deltaCol;
            
            if (row >= BoardSize || col >= BoardSize || col < 0 || Board[row, col] != firstCell)
                return GameResult.None;
        }

        return firstCell == player1CellState ? GameResult.Player1Wins : GameResult.Player2Wins;
    }
} 