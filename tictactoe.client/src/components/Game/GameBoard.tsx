import type { GameStateHook } from '../../hooks/useGameState';
import { CHARACTER_ICONS } from '../../types/game';
import '../../theme/components/game/GameBoard.css';

interface GameBoardProps {
  gameState: GameStateHook;
}

const GameBoard = ({ gameState }: GameBoardProps) => {
  const { currentGame, makeMove, canMakeMove, isMyTurn, me, error } = gameState;

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
      value ? 'filled' : '',
      isMyTurn && isClickable ? 'my-turn' : ''
    ].filter(Boolean).join(' ');
  };

  const getCellContent = (row: number, col: number) => {
    const cellValue = currentGame?.board[row][col];
    
    if (!cellValue || cellValue === '' || cellValue === 'Empty') {
      return null;
    }

    // Find the character info based on the cell value
    const characterInfo = CHARACTER_ICONS.find(char => char.icon === cellValue);
    
    if (characterInfo) {
      return (
        <img
          src={`/game_icons/${characterInfo.fileName}`}
          alt={characterInfo.displayName}
          className="cell-icon"
        />
      );
    }

    // No fallback - all cells should use character icons
    return null;
  };

  const getPlayerCharacterInfo = (playerId: string) => {
    const player = currentGame?.player1?.id === playerId ? currentGame?.player1 : 
                   currentGame?.player2?.id === playerId ? currentGame?.player2 : null;
    
    if (!player) return null;
    
    return CHARACTER_ICONS.find(char => char.icon === player.characterIcon);
  };

  if (!currentGame) {
    return null;
  }

  const myCharacterInfo = me ? CHARACTER_ICONS.find(char => char.icon === me.characterIcon) : null;

  return (
    <div className="game-board">
      <div className="board-container">
        <div className="board">
          {currentGame.board.map((row, rowIndex) =>
            row.map((_, colIndex) => (
              <div
                key={`${rowIndex}-${colIndex}`}
                className={getCellClass(rowIndex, colIndex)}
                onClick={() => handleCellClick(rowIndex, colIndex)}
              >
                <div className="cell-content">
                  {getCellContent(rowIndex, colIndex)}
                </div>
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
                <img
                  src={`/game_icons/${myCharacterInfo?.fileName}`}
                  alt={myCharacterInfo?.displayName}
                  className="turn-indicator-icon"
                />
                Your turn ({myCharacterInfo?.displayName})
              </span>
            ) : (
              <span className="opponent-turn">
                {currentGame.currentPlayer ? (
                  <>
                    <img
                      src={`/game_icons/${getPlayerCharacterInfo(currentGame.currentPlayer.id)?.fileName}`}
                      alt={getPlayerCharacterInfo(currentGame.currentPlayer.id)?.displayName}
                      className="turn-indicator-icon"
                    />
                    {currentGame.currentPlayer.name}'s turn
                  </>
                ) : (
                  'Opponent\'s turn'
                )}
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