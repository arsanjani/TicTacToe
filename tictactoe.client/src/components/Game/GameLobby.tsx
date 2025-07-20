import { useState } from 'react';
import type { GameStateHook } from '../../hooks/useGameState';
import { CHARACTER_ICONS, CharacterIcon } from '../../types/game';
import '../../theme/components/game/GameLobby.css';

interface GameLobbyProps {
  gameState: GameStateHook;
}

const GameLobby = ({ gameState }: GameLobbyProps) => {
  const { createGame, isConnected, error } = gameState;
  const [playerName, setPlayerName] = useState('');
  const [selectedCharacter, setSelectedCharacter] = useState<CharacterIcon>(CharacterIcon.Cross);
  const [boardSize, setBoardSize] = useState(3);
  const [isCreating, setIsCreating] = useState(false);
  const [isPrivate, setIsPrivate] = useState(false);
  const [playWithAI, setPlayWithAI] = useState(false);

  const handleCreateGame = async () => {
    if (!playerName.trim()) return;
    
    setIsCreating(true);
    try {
      await createGame(playerName.trim(), selectedCharacter, boardSize, isPrivate, playWithAI);
    } finally {
      setIsCreating(false);
    }
  };

  return (
    <div className="lobby-game-lobby">
      <div className="lobby-container">
        <div className="lobby-header">
          <h1>🎮 <span>Tic Tac Toe</span></h1>
          <p className="lobby-subtitle">Challenge your friends to a classic game!</p>
        </div>

        <div className="lobby-create-game-section">
          <h2>Create New Game</h2>
          <div className="lobby-player-setup">
            <div className="lobby-input-group">
              <label htmlFor="playerName">Your Name</label>
              <input
                id="playerName"
                type="text"
                placeholder="Enter your name"
                value={playerName}
                onChange={(e) => setPlayerName(e.target.value)}
                className="lobby-player-name-input"
                maxLength={20}
                disabled={!isConnected}
              />
            </div>

            <div className="lobby-input-group">
              <label>Choose Your Character</label>
              <div className="lobby-character-selection">
                {CHARACTER_ICONS.map((characterInfo) => (
                  <div
                    key={characterInfo.icon}
                    className={`lobby-character-option ${selectedCharacter === characterInfo.icon ? 'selected' : ''}`}
                    onClick={() => setSelectedCharacter(characterInfo.icon)}
                  >
                    <img
                      src={`/game_icons/${characterInfo.fileName}`}
                      alt={characterInfo.displayName}
                      className="lobby-character-icon"
                    />
                    <span className="lobby-character-name">{characterInfo.displayName}</span>
                  </div>
                ))}
              </div>
            </div>
            
            <div className="lobby-input-group">
              <label>Board Size</label>
              <div className="lobby-board-size-selection">
                <div
                  className={`lobby-board-size-option ${boardSize === 3 ? 'selected' : ''}`}
                  onClick={() => setBoardSize(3)}
                >
                  <div className="lobby-board-preview">
                    <div className="lobby-preview-grid grid-3x3">
                      {Array.from({ length: 9 }).map((_, i) => (
                        <div key={i} className="lobby-preview-cell"></div>
                      ))}
                    </div>
                  </div>
                  <span className="lobby-board-size-label">3x3 Classic</span>
                </div>
                <div
                  className={`lobby-board-size-option ${boardSize === 4 ? 'selected' : ''}`}
                  onClick={() => setBoardSize(4)}
                >
                  <div className="lobby-board-preview">
                    <div className="lobby-preview-grid grid-4x4">
                      {Array.from({ length: 16 }).map((_, i) => (
                        <div key={i} className="lobby-preview-cell"></div>
                      ))}
                    </div>
                  </div>
                  <span className="lobby-board-size-label">4x4 Extended</span>
                </div>
              </div>
            </div>
            
            <div className="lobby-input-group">
              <label htmlFor="aiSwitch" className="lobby-switch-label">
                <span>Play with AI</span>
                <div className="lobby-switch-container">
                  <input
                    id="aiSwitch"
                    type="checkbox"
                    checked={playWithAI}
                    onChange={(e) => setPlayWithAI(e.target.checked)}
                    className="lobby-switch-input"
                    disabled={!isConnected}
                  />
                  <div className="lobby-switch-slider"></div>
                </div>
              </label>
              <p className="lobby-switch-description">
                {playWithAI ? 'Start game immediately against AI opponent' : 'Wait for another player to join'}
              </p>
            </div>
            
            {!playWithAI && (
              <div className="lobby-input-group">
                <label htmlFor="privateSwitch" className="lobby-switch-label">
                  <span>Private Game</span>
                  <div className="lobby-switch-container">
                    <input
                      id="privateSwitch"
                      type="checkbox"
                      checked={isPrivate}
                      onChange={(e) => setIsPrivate(e.target.checked)}
                      className="lobby-switch-input"
                      disabled={!isConnected}
                    />
                    <div className="lobby-switch-slider"></div>
                  </div>
                </label>
                <p className="lobby-switch-description">
                  {isPrivate ? 'Only accessible via share link' : 'Visible in public game list'}
                </p>
              </div>
            )}
            
            <button
              onClick={handleCreateGame}
              disabled={!playerName.trim() || isCreating || !isConnected}
              className="lobby-create-game-button"
            >
              {isCreating ? 'Creating...' : 'Create Game'}
            </button>
          </div>
        </div>

        {error && (
          <div className="lobby-error-message">
            <p>{error}</p>
          </div>
        )}
      </div>
    </div>
  );
};

export default GameLobby; 