import { useState, useEffect, useCallback } from 'react';
import { gameService } from '../services/gameService';
import { GameState, GameResult } from '../types/game';
import type { 
  Player, 
  Game, 
  GameListItem, 
  GameCreatedEvent, 
  GameStartedEvent, 
  MoveMadeEvent, 
  GameFinishedEvent, 
  GameResetEvent, 
  PlayerDisconnectedEvent, 
  GameError 
} from '../types/game';

export interface GameStateHook {
  // Game state
  currentGame: Game | null;
  waitingGames: GameListItem[];
  connectionState: string;
  error: string | null;
  isConnected: boolean;
  currentPlayerId: string | null;
  
  // Game actions
  connect: () => Promise<void>;
  disconnect: () => Promise<void>;
  createGame: (playerName: string) => Promise<void>;
  joinGame: (gameId: string, playerName: string) => Promise<void>;
  makeMove: (row: number, col: number) => Promise<void>;
  getWaitingGames: () => Promise<void>;
  resetGame: () => Promise<void>;
  clearError: () => void;
  
  // Game info
  isMyTurn: boolean;
  canMakeMove: (row: number, col: number) => boolean;
  isGameInProgress: boolean;
  isGameFinished: boolean;
  isWaitingForPlayer: boolean;
  mySymbol: 'X' | 'O' | null;
  opponent: Player | null;
  me: Player | null;
}

export const useGameState = (): GameStateHook => {
  const [currentGame, setCurrentGame] = useState<Game | null>(null);
  const [waitingGames, setWaitingGames] = useState<GameListItem[]>([]);
  const [connectionState, setConnectionState] = useState<string>('Disconnected');
  const [error, setError] = useState<string | null>(null);
  const [isConnected, setIsConnected] = useState(false);
  const [currentPlayerId, setCurrentPlayerId] = useState<string | null>(null);

  // Event handlers
  const handleGameCreated = useCallback((event: GameCreatedEvent) => {
    setCurrentGame({
      id: event.gameId,
      player1: { 
        id: event.playerId, 
        name: event.playerName, 
        symbol: event.symbol,
        isReady: true,
        isActive: true,
        joinedAt: new Date()
      },
      player2: null,
      board: Array(3).fill(null).map(() => Array(3).fill('')),
      state: GameState.WaitingForPlayers,
      result: GameResult.None,
      currentPlayer: null,
      createdAt: new Date(),
      startedAt: null,
      endedAt: null,
      moveCount: 0
    });
  }, []);

  const handleGameStarted = useCallback((event: GameStartedEvent) => {
    setCurrentGame(prev => {
      if (prev) {
        // We already have a game (creator). Just update the details.
        return {
          ...prev,
          player1: event.player1,
          player2: event.player2,
          currentPlayer: event.currentPlayer,
          state: GameState.InProgress,
          startedAt: new Date()
        };
      }

      // No existing game (joiner). Create a fresh game instance.
      return {
        id: event.gameId,
        player1: event.player1,
        player2: event.player2,
        board: Array(3).fill(null).map(() => Array(3).fill('')),
        state: GameState.InProgress,
        result: GameResult.None,
        currentPlayer: event.currentPlayer,
        createdAt: new Date(), // We don't have the original timestamp; use "now" for reference
        startedAt: new Date(),
        endedAt: null,
        moveCount: 0
      };
    });
  }, []);

  const handleMoveMade = useCallback((event: MoveMadeEvent) => {
    setCurrentGame(prev => prev ? {
      ...prev,
      board: event.board,
      currentPlayer: event.currentPlayer,
      moveCount: event.moveCount
    } : null);
  }, []);

  const handleGameFinished = useCallback((event: GameFinishedEvent) => {
    setCurrentGame(prev => prev ? {
      ...prev,
      state: GameState.Finished,
      result: event.result as GameResult,
      endedAt: new Date(event.endedAt)
    } : null);
  }, []);

  const handleGameReset = useCallback((event: GameResetEvent) => {
    setCurrentGame(prev => prev ? {
      ...prev,
      board: event.board,
      currentPlayer: event.currentPlayer,
      state: GameState.InProgress,
      result: GameResult.None,
      endedAt: null,
      moveCount: 0
    } : null);
  }, []);

  const handlePlayerDisconnected = useCallback((event: PlayerDisconnectedEvent) => {
    setError(`Player disconnected from game ${event.gameId}`);
    setCurrentGame(prev => prev ? {
      ...prev,
      state: GameState.Finished
    } : null);
  }, []);

  const handleWaitingGames = useCallback((games: GameListItem[]) => {
    setWaitingGames(games);
  }, []);

  const handleError = useCallback((error: GameError) => {
    setError(error.message);
  }, []);

  const handleConnectionEstablished = useCallback((connectionId: string) => {
    setCurrentPlayerId(connectionId);
    console.log('Connection established with ID:', connectionId);
  }, []);

  // Setup event handlers
  useEffect(() => {
    gameService.onGameCreated = handleGameCreated;
    gameService.onGameStarted = handleGameStarted;
    gameService.onMoveMade = handleMoveMade;
    gameService.onGameFinished = handleGameFinished;
    gameService.onGameReset = handleGameReset;
    gameService.onPlayerDisconnected = handlePlayerDisconnected;
    gameService.onWaitingGames = handleWaitingGames;
    gameService.onError = handleError;
    gameService.onConnectionEstablished = handleConnectionEstablished;

    return () => {
      // Cleanup
      gameService.onGameCreated = undefined;
      gameService.onGameStarted = undefined;
      gameService.onMoveMade = undefined;
      gameService.onGameFinished = undefined;
      gameService.onGameReset = undefined;
      gameService.onPlayerDisconnected = undefined;
      gameService.onWaitingGames = undefined;
      gameService.onError = undefined;
      gameService.onConnectionEstablished = undefined;
    };
  }, [handleGameCreated, handleGameStarted, handleMoveMade, handleGameFinished, handleGameReset, handlePlayerDisconnected, handleWaitingGames, handleError, handleConnectionEstablished]);

  // Connection management
  const connect = useCallback(async () => {
    try {
      await gameService.connect();
      setIsConnected(true);
      setConnectionState(gameService.getConnectionState());
      setError(null);
    } catch (error) {
      setError(`Failed to connect: ${error}`);
    }
  }, []);

  const disconnect = useCallback(async () => {
    try {
      await gameService.disconnect();
      setIsConnected(false);
      setConnectionState(gameService.getConnectionState());
      setCurrentGame(null);
      setCurrentPlayerId(null);
    } catch (error) {
      setError(`Failed to disconnect: ${error}`);
    }
  }, []);

  // Game actions
  const createGame = useCallback(async (playerName: string) => {
    try {
      await gameService.createGame(playerName);
      setError(null);
    } catch (error) {
      setError(`Failed to create game: ${error}`);
    }
  }, []);

  const joinGame = useCallback(async (gameId: string, playerName: string) => {
    try {
      await gameService.joinGame(gameId, playerName);
      setError(null);
    } catch (error) {
      setError(`Failed to join game: ${error}`);
    }
  }, []);

  const makeMove = useCallback(async (row: number, col: number) => {
    if (!currentGame) return;
    
    try {
      await gameService.makeMove(currentGame.id, row, col);
      setError(null);
    } catch (error) {
      setError(`Failed to make move: ${error}`);
    }
  }, [currentGame]);

  const getWaitingGames = useCallback(async () => {
    try {
      await gameService.getWaitingGames();
      setError(null);
    } catch (error) {
      setError(`Failed to get waiting games: ${error}`);
    }
  }, []);

  const resetGame = useCallback(async () => {
    if (!currentGame) return;
    
    try {
      await gameService.resetGame(currentGame.id);
      setError(null);
    } catch (error) {
      setError(`Failed to reset game: ${error}`);
    }
  }, [currentGame]);

  const clearError = useCallback(() => {
    setError(null);
  }, []);

  // Computed properties
  const isMyTurn = currentGame?.currentPlayer?.id === currentPlayerId;
  const canMakeMove = (row: number, col: number): boolean => {
    return Boolean(
      currentGame &&
      currentGame.state === GameState.InProgress &&
      isMyTurn &&
      currentGame.board[row][col] === ''
    );
  };
  
  const isGameInProgress = currentGame?.state === GameState.InProgress;
  const isGameFinished = currentGame?.state === GameState.Finished;
  const isWaitingForPlayer = currentGame?.state === GameState.WaitingForPlayers;
  
  const mySymbol = currentGame?.player1?.id === currentPlayerId ? 
    currentGame.player1.symbol : 
    currentGame?.player2?.id === currentPlayerId ? 
    currentGame.player2.symbol : 
    null;
    
  const opponent = currentGame?.player1?.id === currentPlayerId ? 
    currentGame.player2 : 
    currentGame?.player2?.id === currentPlayerId ? 
    currentGame.player1 : 
    null;
    
  const me = currentGame?.player1?.id === currentPlayerId ? 
    currentGame.player1 : 
    currentGame?.player2?.id === currentPlayerId ? 
    currentGame.player2 : 
    null;

  return {
    currentGame,
    waitingGames,
    connectionState,
    error,
    isConnected,
    currentPlayerId,
    connect,
    disconnect,
    createGame,
    joinGame,
    makeMove,
    getWaitingGames,
    resetGame,
    clearError,
    isMyTurn,
    canMakeMove,
    isGameInProgress,
    isGameFinished,
    isWaitingForPlayer,
    mySymbol,
    opponent,
    me
  };
}; 