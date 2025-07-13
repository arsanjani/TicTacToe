import type { GameStateHook } from '../../hooks/useGameState';
import '../../theme/components/game/GameBoard.css';

interface GameBoardProps {
  gameState: GameStateHook;
}

const GameBoard = ({ gameState }: GameBoardProps) => {
  const { currentGame, makeMove, canMakeMove, isMyTurn, mySymbol, error } = gameState;

  const handleCellClick = (row: number, col: number) => {
    if (canMakeMove(row, col)) {
      makeMove(row, col);
    }
  };

  const getCellClass = (row: number, col: number) => {
    const baseClass = 'cell';
    const isClickable = canMakeMove(row, col);
    const value = currentGame?.board[row][col];
    
    return [
      baseClass,
      isClickable ? 'clickable' : '',
      value === 'X' ? 'x' : value === 'O' ? 'o' : '',
      isMyTurn && isClickable ? 'my-turn' : ''
    ].filter(Boolean).join(' ');
  };

  if (!currentGame) {
    return null;
  }

  return (
    <div className="game-board">
      <div className="board-container">
        <div className="board">
          {currentGame.board.map((row, rowIndex) =>
            row.map((cell, colIndex) => (
              <div
                key={`${rowIndex}-${colIndex}`}
                className={getCellClass(rowIndex, colIndex)}
                onClick={() => handleCellClick(rowIndex, colIndex)}
              >
                <span className="cell-content">{cell}</span>
              </div>
            ))
          )}
        </div>
      </div>
      
      <div className="game-info">
        <div className="player-info">
          <div className="current-player">
            {isMyTurn ? (
              <span className="my-turn-indicator">
                Your turn ({mySymbol})
              </span>
            ) : (
              <span className="opponent-turn">
                Opponent's turn
              </span>
            )}
          </div>
        </div>
      </div>

      {error && currentGame.state === 'Finished' && (
        <div className="disconnection-overlay">
          <div className="disconnection-message">
            <div className="disconnection-icon">😱</div>
            <h2>{error}</h2>
            <button onClick={() => window.location.href = '/'} className="back-to-home-button">
              Go to Home Page
            </button>
          </div>
        </div>
      )}
    </div>
  );
};

export default GameBoard; 