import { GameResult as GameResultType } from '../../types/game';
import type { GameStateHook } from '../../hooks/useGameState';
import '../../theme/components/game/GameResult.css';

interface GameResultProps {
  gameState: GameStateHook;
}

const GameResult = ({ gameState }: GameResultProps) => {
  const { currentGame, resetGame, me, opponent } = gameState;

  if (!currentGame || !currentGame.endedAt) {
    return null;
  }

  const getResultMessage = () => {
    switch (currentGame.result) {
      case GameResultType.Player1Wins:
        return currentGame.player1?.id === me?.id ? 'You Win!' : `${opponent?.name} Wins!`;
      case GameResultType.Player2Wins:
        return currentGame.player2?.id === me?.id ? 'You Win!' : `${opponent?.name} Wins!`;
      case GameResultType.Draw:
        return "It's a Draw!";
      default:
        return 'Game Over';
    }
  };

  const getResultClass = () => {
    switch (currentGame.result) {
      case GameResultType.Player1Wins:
        return currentGame.player1?.id === me?.id ? 'win' : 'lose';
      case GameResultType.Player2Wins:
        return currentGame.player2?.id === me?.id ? 'win' : 'lose';
      case GameResultType.Draw:
        return 'draw';
      default:
        return '';
    }
  };

  const getResultIcon = () => {
    switch (currentGame.result) {
      case GameResultType.Player1Wins:
        return currentGame.player1?.id === me?.id ? '🏆' : '😔';
      case GameResultType.Player2Wins:
        return currentGame.player2?.id === me?.id ? '🏆' : '😔';
      case GameResultType.Draw:
        return '🤝';
      default:
        return '🎮';
    }
  };

  const handlePlayAgain = () => {
    resetGame();
  };

  return (
    <div className="game-result-overlay">
      <div className="game-result-modal">
        <div className={`result-content ${getResultClass()}`}>
          <div className="result-icon">
            {getResultIcon()}
          </div>
          
          <h2 className="result-title">
            {getResultMessage()}
          </h2>
          
          <div className="game-stats">
            <p>Total moves: {currentGame.moveCount}</p>
            <p>Game duration: {Math.round((new Date(currentGame.endedAt).getTime() - new Date(currentGame.startedAt || 0).getTime()) / 1000)}s</p>
          </div>
          
          <div className="players-summary">
            <div className="player-card">
              <div className="player-symbol x">X</div>
              <div className="player-name">{currentGame.player1?.name}</div>
            </div>
            <div className="vs">VS</div>
            <div className="player-card">
              <div className="player-symbol o">O</div>
              <div className="player-name">{currentGame.player2?.name}</div>
            </div>
          </div>
          
          <div className="result-actions">
            <button onClick={handlePlayAgain} className="play-again-button">
              🔄 Play Again
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default GameResult; 