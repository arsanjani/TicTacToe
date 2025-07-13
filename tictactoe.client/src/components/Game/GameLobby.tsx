import { useState } from 'react';
import type { GameStateHook } from '../../hooks/useGameState';
import '../../theme/components/game/GameLobby.css';

interface GameLobbyProps {
  gameState: GameStateHook;
}

const GameLobby = ({ gameState }: GameLobbyProps) => {
  const { createGame, isConnected, error } = gameState;
  const [playerName, setPlayerName] = useState('');
  const [isCreating, setIsCreating] = useState(false);

  const handleCreateGame = async () => {
    if (!playerName.trim()) return;
    
    setIsCreating(true);
    try {
      await createGame(playerName.trim());
    } finally {
      setIsCreating(false);
    }
  };

  return (
    <div className="game-lobby">
      <div className="lobby-container">
        <div className="lobby-header">
          <h1>🎮 Tic Tac Toe</h1>
          <p className="lobby-subtitle">Challenge your friends to a classic game!</p>
        </div>

        <div className="lobby-content">
          <div className="create-game-section">
            <h2>Create New Game</h2>
            <div className="player-setup">
              <div className="input-group">
                <label htmlFor="playerName">Your Name</label>
                <input
                  id="playerName"
                  type="text"
                  placeholder="Enter your name"
                  value={playerName}
                  onChange={(e) => setPlayerName(e.target.value)}
                  className="player-name-input"
                  maxLength={20}
                  disabled={!isConnected}
                />
              </div>
              
              <button
                onClick={handleCreateGame}
                disabled={!playerName.trim() || isCreating || !isConnected}
                className="create-game-button"
              >
                {isCreating ? 'Creating...' : 'Create Game'}
              </button>
            </div>
          </div>

          <div className="game-rules">
            <h3>How to Play</h3>
            <ul>
              <li>🎯 Get three of your symbols in a row (horizontal, vertical, or diagonal)</li>
              <li>🎮 X goes first, then players alternate turns</li>
              <li>🤝 Share your game link with a friend to play together</li>
              <li>🏆 First to get three in a row wins!</li>
            </ul>
          </div>

          {error && (
            <div className="error-message">
              <p>{error}</p>
            </div>
          )}

          {!isConnected && (
            <div className="connection-status">
              <p>Connecting to game server...</p>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default GameLobby; 