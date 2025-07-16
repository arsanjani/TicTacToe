import { useState, useEffect } from 'react';
import type { GameStateHook } from '../../hooks/useGameState';
import { CHARACTER_ICONS, CharacterIcon } from '../../types/game';
import { shouldUseWebShare, shareContent, copyToClipboard } from '../../utils/deviceUtils';
import '../../theme/components/game/WaitingRoom.css';

interface WaitingRoomProps {
  gameState: GameStateHook;
  joinGameId?: string | null;
}

const WaitingRoom = ({ gameState, joinGameId }: WaitingRoomProps) => {
  const { currentGame, waitingGames, getWaitingGames, joinGame, me } = gameState;
  const [playerName, setPlayerName] = useState('');
  const [selectedCharacter, setSelectedCharacter] = useState<CharacterIcon>(CharacterIcon.Circle);
  const [isJoining, setIsJoining] = useState(false);
  const [isSharing, setIsSharing] = useState(false);
  const [showToast, setShowToast] = useState(false);
  const [toastExiting, setToastExiting] = useState(false);
  const useMobileShare = shouldUseWebShare();

  useEffect(() => {
    // Refresh waiting games every 3 seconds
    const interval = setInterval(() => {
      getWaitingGames();
    }, 3000);

    // Initial fetch
    getWaitingGames();

    return () => clearInterval(interval);
  }, [getWaitingGames]);

  // Auto-hide toast after 3 seconds
  useEffect(() => {
    if (showToast) {
      const timer = setTimeout(() => {
        hideToast();
      }, 3000);

      return () => clearTimeout(timer);
    }
  }, [showToast]);

  const hideToast = () => {
    setToastExiting(true);
    setTimeout(() => {
      setShowToast(false);
      setToastExiting(false);
    }, 300); // Match the CSS animation duration
  };

  const showToastMessage = () => {
    setShowToast(true);
    setToastExiting(false);
  };

  const handleJoinGame = async (gameId: string) => {
    if (!playerName.trim()) return;
    
    setIsJoining(true);
    try {
      await joinGame(gameId, playerName.trim(), selectedCharacter);
    } finally {
      setIsJoining(false);
    }
  };

  const getShareableLink = () => {
    if (!currentGame) return '';
    return `${window.location.origin}?gameId=${currentGame.id}`;
  };

  const handleShareOrCopy = async () => {
    const link = getShareableLink();
    setIsSharing(true);
    
    try {
      if (useMobileShare) {
        // Use Web Share API on mobile devices
        const shared = await shareContent({
          title: 'Join my Tic Tac Toe game!',
          text: `Join me for a game of Tic Tac Toe! Click the link to play.`,
          url: link
        });
        
        if (!shared) {
          // User cancelled sharing, fallback to clipboard
          await copyToClipboard(link);
          showToastMessage();
        }
      } else {
        // Use clipboard copy on desktop
        await copyToClipboard(link);
        showToastMessage();
      }
    } catch (err) {
      console.error('Failed to share or copy: ', err);
      // Try clipboard as fallback
      try {
        await copyToClipboard(link);
        showToastMessage();
      } catch (clipboardErr) {
        console.error('Clipboard fallback also failed: ', clipboardErr);
      }
    } finally {
      setIsSharing(false);
    }
  };

  // Filter waiting games based on joinGameId if provided
  const filteredWaitingGames = joinGameId 
    ? waitingGames.filter(game => game.gameId === joinGameId)
    : waitingGames;

  // Show waiting room for game creator
  if (currentGame && currentGame.state === 'WaitingForPlayers') {
    return (
      <div className="waiting-room">
        <div className="waiting-container">
          <div className="waiting-header">
            <h2>🎮 Game Created!</h2>
            <p className="waiting-subtitle">Share the link below to invite a friend</p>
          </div>

          <div className="waiting-game-info">
            <div className="waiting-player-info">
              <div className="waiting-player-card">
                <div className="waiting-player-avatar">
                  <img
                    src={`/game_icons/${CHARACTER_ICONS.find(c => c.icon === me?.characterIcon)?.fileName}`}
                    alt={CHARACTER_ICONS.find(c => c.icon === me?.characterIcon)?.displayName}
                    className="waiting-player-icon"
                  />
                </div>
                <div className="waiting-player-details">
                  <h3>{me?.name || 'You'}</h3>
                  <p>Playing as {CHARACTER_ICONS.find(c => c.icon === me?.characterIcon)?.displayName}</p>
                </div>
              </div>
            </div>

            <div className="waiting-share-section">
              <div className="waiting-share-link">
                <input
                  type="text"
                  value={getShareableLink()}
                  readOnly
                  className="waiting-share-input"
                />
                <button 
                  onClick={handleShareOrCopy} 
                  disabled={isSharing}
                  className="waiting-copy-button"
                >
                  {isSharing 
                    ? '⏳ Sharing...' 
                    : useMobileShare 
                      ? '🔗 Share' 
                      : '📋 Copy'
                  }
                </button>
              </div>
              <p className="waiting-share-description">
                Share this link with your friend so they can join your game!
              </p>
            </div>
          </div>

          <div className="waiting-status">
            <div className="waiting-spinner"></div>
            <p>Waiting for opponent to join...</p>
          </div>

          {/* Toast Notification */}
          {showToast && (
            <div className={`waiting-toast ${toastExiting ? 'waiting-toast-exit' : ''}`}>
              <div className="waiting-toast-icon">✅</div>
              <div className="waiting-toast-content">
                <div className="waiting-toast-title">Link Copied!</div>
                <div className="waiting-toast-message">Game link has been copied to clipboard</div>
              </div>
            </div>
          )}
        </div>
      </div>
    );
  }

  // Show join game interface
  return (
    <div className="waiting-room">
      <div className="waiting-container">
        <div className="waiting-header">
          <h2>🎯 Join a Game</h2>
          <p className="waiting-subtitle">Choose a game to join and start playing!</p>
        </div>

        <div className="waiting-join-section">
          <div className="waiting-player-setup">
            <div className="waiting-input-group">
              <label htmlFor="joinPlayerName">Your Name</label>
              <input
                id="joinPlayerName"
                type="text"
                placeholder="Enter your name"
                value={playerName}
                onChange={(e) => setPlayerName(e.target.value)}
                className="waiting-player-name-input"
                maxLength={20}
              />
            </div>

            <div className="waiting-input-group">
              <label>Choose Your Character</label>
              <div className="waiting-character-selection">
                {CHARACTER_ICONS.map((characterInfo) => (
                  <div
                    key={characterInfo.icon}
                    className={`waiting-character-option ${selectedCharacter === characterInfo.icon ? 'selected' : ''}`}
                    onClick={() => setSelectedCharacter(characterInfo.icon)}
                  >
                    <img
                      src={`/game_icons/${characterInfo.fileName}`}
                      alt={characterInfo.displayName}
                      className="waiting-character-icon"
                    />
                    <span className="waiting-character-name">{characterInfo.displayName}</span>
                  </div>
                ))}
              </div>
            </div>
          </div>

          <div className="waiting-games-section">
            <h3>Available Games</h3>
            {filteredWaitingGames.length === 0 ? (
              <div className="waiting-no-games">
                <p>No games available at the moment.</p>
                <p>Create a game to start playing!</p>
              </div>
            ) : (
              <div className="waiting-games-grid">
                {filteredWaitingGames.map((game) => (
                  <div key={game.gameId} className="waiting-game-card">
                    <div className="waiting-game-card-header">
                      <div className="waiting-game-creator">
                        <img
                          src={`/game_icons/${CHARACTER_ICONS.find(c => c.icon === game.player1CharacterIcon)?.fileName}`}
                          alt={CHARACTER_ICONS.find(c => c.icon === game.player1CharacterIcon)?.displayName}
                          className="waiting-game-creator-icon"
                        />
                        <div>
                          <h4>{game.player1Name}</h4>
                          <p>Playing as {CHARACTER_ICONS.find(c => c.icon === game.player1CharacterIcon)?.displayName}</p>
                        </div>
                      </div>
                      <span className="waiting-indicator">Waiting...</span>
                    </div>
                    <div className="waiting-game-card-info">
                      <p>Created: {new Date(game.createdAt).toLocaleTimeString()}</p>
                      {joinGameId && (
                        <p>Game ID: <span className="waiting-game-id">{game.gameId}</span></p>
                      )}
                      {game.isPrivate && (
                        <p className="waiting-private-indicator">🔒 Private Game</p>
                      )}
                    </div>
                    {game.isPrivate && !joinGameId ? (
                      <div className="waiting-private-game-indicator">
                        <div className="waiting-lock-icon">🔒</div>
                        <p>Private game - join via share link only</p>
                      </div>
                    ) : (
                      <button
                        onClick={() => handleJoinGame(game.gameId)}
                        disabled={!playerName.trim() || isJoining || selectedCharacter === game.player1CharacterIcon}
                        className="waiting-join-button"
                      >
                        {selectedCharacter === game.player1CharacterIcon ? 'Character Taken' : 
                         isJoining ? 'Joining...' : 'Join Game'}
                      </button>
                    )}
                  </div>
                ))}
              </div>
            )}
          </div>

          {!joinGameId && (
            <div className="waiting-refresh-section">
              <button onClick={getWaitingGames} className="waiting-refresh-button">
                Refresh Games
              </button>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default WaitingRoom; 