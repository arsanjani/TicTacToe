import { useState, useEffect } from 'react';
import type { GameStateHook } from '../../hooks/useGameState';
import '../../theme/components/game/WaitingRoom.css';

interface WaitingRoomProps {
  gameState: GameStateHook;
}

const WaitingRoom = ({ gameState }: WaitingRoomProps) => {
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

  if (currentGame?.state === 'WaitingForPlayers') {
    return (
      <div className="waiting-room">
        <div className="waiting-container">
          <div className="waiting-header">
            <h2>Waiting for Player...</h2>
            <div className="spinner">
              <div className="spinner-circle"></div>
            </div>
          </div>
          
          <div className="game-info">
            <p>You are <strong>{me?.name}</strong> playing as <strong>{me?.symbol}</strong></p>
            <p>Game ID: <span className="game-id">{currentGame.id}</span></p>
          </div>

          <div className="share-section">
            <h3>Share this game with a friend:</h3>
            <div className="share-link">
              <input
                type="text"
                value={getShareableLink()}
                readOnly
                className="share-input"
              />
              <button onClick={copyToClipboard} className="copy-button">
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
        <div className="join-section">
          <h2>Join a Game</h2>
          <div className="name-input">
            <input
              type="text"
              placeholder="Enter your name"
              value={playerName}
              onChange={(e) => setPlayerName(e.target.value)}
              className="player-name-input"
              maxLength={20}
            />
          </div>
        </div>

        <div className="games-list">
          <h3>Available Games</h3>
          {waitingGames.length === 0 ? (
            <div className="no-games">
              <p>No games available right now.</p>
              <p>Create a new game to start playing!</p>
            </div>
          ) : (
            <div className="games-grid">
              {waitingGames.map((game) => (
                <div key={game.gameId} className="game-card">
                  <div className="game-card-header">
                    <h4>{game.player1Name}</h4>
                    <span className="waiting-indicator">Waiting...</span>
                  </div>
                  <div className="game-card-info">
                    <p>Created: {new Date(game.createdAt).toLocaleTimeString()}</p>
                  </div>
                  <button
                    onClick={() => handleJoinGame(game.gameId)}
                    disabled={!playerName.trim() || isJoining}
                    className="join-button"
                  >
                    {isJoining ? 'Joining...' : 'Join Game'}
                  </button>
                </div>
              ))}
            </div>
          )}
        </div>

        <div className="refresh-section">
          <button onClick={getWaitingGames} className="refresh-button">
            Refresh Games
          </button>
        </div>
      </div>
    </div>
  );
};

export default WaitingRoom; 