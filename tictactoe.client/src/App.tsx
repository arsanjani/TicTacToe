import { useEffect, useState } from 'react';
import { useGameState } from './hooks/useGameState';
import { GameState } from './types/game';
import GameLobby from './components/Game/GameLobby';
import WaitingRoom from './components/Game/WaitingRoom';
import GameBoard from './components/Game/GameBoard';
import GameResult from './components/Game/GameResult';
import './theme/global/app.css';

function App() {
    const gameState = useGameState();
    const [isConnecting, setIsConnecting] = useState(true);
    const [isJoinMode, setIsJoinMode] = useState(false);
    const [joinGameId, setJoinGameId] = useState<string | null>(null);

    useEffect(() => {
        // Check if there's a gameId in the URL for direct joining
        const urlParams = new URLSearchParams(window.location.search);
        const gameId = urlParams.get('gameId');
        if (gameId) {
            setIsJoinMode(true);
            setJoinGameId(gameId);
        }

        // Connect to the game hub when the app starts
        gameState.connect()
            .then(() => {
                setIsConnecting(false);
                
                if (gameId) {
                    // You could implement auto-join logic here
                    console.log('Game ID found in URL:', gameId);
                }
            })
            .catch((error) => {
                console.error('Failed to connect:', error);
                setIsConnecting(false);
            });

        // Cleanup on unmount
        return () => {
            gameState.disconnect();
        };
    }, []);

    // Show loading while connecting
    if (isConnecting) {
        return (
            <div className="app-loading">
                <div className="loading-content">
                    <div className="spinner"></div>
                    <p>Connecting to game server...</p>
                </div>
            </div>
        );
    }

    // Show error message if there's a connection error
    if (gameState.error && !gameState.isConnected) {
        return (
            <div className="app-error">
                <div className="error-content">
                    <h2>Connection Error</h2>
                    <p>{gameState.error}</p>
                    <button onClick={() => window.location.reload()}>
                        Retry
                    </button>
                </div>
            </div>
        );
    }

    // Main game flow
    if (!gameState.currentGame) {
        return (
            <div className="app">
                {isJoinMode ? (
                    // Show only Join a Game card when coming from join link
                    <WaitingRoom gameState={gameState} joinGameId={joinGameId} />
                ) : (
                    // Show both cards for normal visits
                    <>
                        <GameLobby gameState={gameState} />
                        <WaitingRoom gameState={gameState} />
                    </>
                )}
            </div>
        );
    }

    // Game is waiting for players
    if (gameState.currentGame.state === GameState.WaitingForPlayers) {
        return (
            <div className="app">
                <WaitingRoom gameState={gameState} />
            </div>
        );
    }

    // Game is in progress or finished
    return (
        <div className="app">
            <div className="game-container">
                <div className="game-header">
                    <h1>🎮 <span>Tic Tac Toe</span></h1>
                    <div className="players-info">
                        <div className="player-info">
                            <span className="player-name">{gameState.currentGame.player1?.name}</span>
                            <span className="player-symbol x">X</span>
                        </div>
                        <div className="vs">VS</div>
                        <div className="player-info">
                            <span className="player-name">{gameState.currentGame.player2?.name}</span>
                            <span className="player-symbol o">O</span>
                        </div>
                    </div>
                </div>
                
                <GameBoard gameState={gameState} />
                
                <div className="game-actions">
                    <button 
                        onClick={() => window.location.reload()} 
                        className="leave-game-button"
                    >
                        Leave Game
                    </button>
                </div>
            </div>
            
            {gameState.isGameFinished && (
                <GameResult gameState={gameState} />
            )}
        </div>
    );
}

export default App;