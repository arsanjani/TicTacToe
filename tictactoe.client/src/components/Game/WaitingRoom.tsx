import { useState, useEffect } from 'react';
import type { GameStateHook } from '../../hooks/useGameState';
import '../../theme/components/game/WaitingRoom.css';

interface WaitingRoomProps {
  gameState: GameStateHook;
  joinGameId?: string | null;
}

const WaitingRoom = ({ gameState, joinGameId }: WaitingRoomProps) => {
  const { currentGame, waitingGames, getWaitingGames, joinGame, me } = gameState;
  const [playerName, setPlayerName] = useState('');
  const [isJoining, setIsJoining] = useState(false);

  useEffect(() => {
    // Refresh waiting games every 3 seconds
    const interval = setInterval(() => {
      getWaitingGames();
    }, 3000);

    // Initial fetch
    getWaitingGames();

    return () => clearInterval(interval);
  }, [getWaitingGames]);

  const handleJoinGame = async (gameId: string) => {
    if (!playerName.trim()) return;
    
    setIsJoining(true);
    try {
      await joinGame(gameId, playerName.trim());
    } finally {
      setIsJoining(false);
    }
  };

  const getShareableLink = () => {
    if (!currentGame) return '';
    return `${window.location.origin}?gameId=${currentGame.id}`;
  };

  const copyToClipboard = async () => {
    try {
      await navigator.clipboard.writeText(getShareableLink());
      // Could add a toast notification here
    } catch (err) {
      console.error('Failed to copy: ', err);
    }
  };

  // Filter waiting games based on joinGameId if provided
  const filteredWaitingGames = joinGameId 
    ? waitingGames.filter(game => game.gameId === joinGameId)
    : waitingGames;

  if (currentGame?.state === 'WaitingForPlayers') {
    return (
      <div className="waiting-room">
        <div className="waiting-container">
          <div className="waiting-header">
            <h2>Waiting for Player...</h2>
            <div className="waiting-spinner">
              <div className="waiting-spinner-circle"></div>
            </div>
          </div>
          
          <div className="waiting-game-info">
            <p>You are <strong>{me?.name}</strong> playing as <strong>{me?.symbol}</strong></p>
            <p>Game ID: <span className="waiting-game-id">{currentGame.id}</span></p>
          </div>

          <div className="waiting-share-section">
            <h3>Share this game with a friend:</h3>
            <div className="waiting-share-link">
              <input
                type="text"
                value={getShareableLink()}
                readOnly
                className="waiting-share-input"
              />
              <button onClick={copyToClipboard} className="waiting-copy-button">
                Copy Link
              </button>
            </div>
          </div>

          <div className="waiting-message">
            <p>Share the link above or wait for someone to join your game!</p>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="waiting-room">
      <div className="waiting-container">
        <div className="waiting-join-section">
          <h2>{joinGameId ? 'Join This Game' : 'Join a Game'}</h2>
          <div className="waiting-name-input">
            <input
              type="text"
              placeholder="Enter your name"
              value={playerName}
              onChange={(e) => setPlayerName(e.target.value)}
              className="waiting-player-name-input"
              maxLength={20}
            />
          </div>
        </div>

        <div className="waiting-games-list">
          <h3>{joinGameId ? 'Game Details' : 'Available Games'}</h3>
          {filteredWaitingGames.length === 0 ? (
            <div className="waiting-no-games">
              {joinGameId ? (
                <>
                  <p>This game is no longer available.</p>
                  <p>It may have already started or been cancelled.</p>
                </>
              ) : (
                <>
                  <p>No games available right now.</p>
                  <p>Create a new game to start playing!</p>
                </>
              )}
            </div>
          ) : (
            <div className="waiting-games-grid">
              {filteredWaitingGames.map((game) => (
                <div key={game.gameId} className="waiting-game-card">
                  <div className="waiting-game-card-header">
                    <h4>{game.player1Name}</h4>
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
                      disabled={!playerName.trim() || isJoining}
                      className="waiting-join-button"
                    >
                      {isJoining ? 'Joining...' : 'Join Game'}
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
  );
};

export default WaitingRoom; 