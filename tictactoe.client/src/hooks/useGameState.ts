import { useState, useEffect, useCallback } from 'react';
import { gameService } from '../services/gameService';
import { GameState, GameResult, CharacterIcon } from '../types/game';
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
  createGame: (playerName: string, characterIcon: CharacterIcon, boardSize?: number, isPrivate?: boolean, playWithAI?: boolean) => Promise<void>;
  joinGame: (gameId: string, playerName: string, characterIcon: CharacterIcon) => Promise<void>;
  makeMove: (row: number, col: number) => Promise<void>;
  getWaitingGames: () => Promise<void>;
  resetGame: () => Promise<void>;
  leaveGame: () => Promise<void>;
  clearError: () => void;
  
  // Game info
  isMyTurn: boolean;
  canMakeMove: (row: number, col: number) => boolean;
  isGameInProgress: boolean;
  isGameFinished: boolean;
  isWaitingForPlayer: boolean;
  myCharacterIcon: CharacterIcon | null;
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
        characterIcon: event.characterIcon,
        isReady: true,
        isActive: true,
        joinedAt: new Date()
      },
      player2: null,
      board: Array(event.boardSize).fill(null).map(() => Array(event.boardSize).fill('')),
      boardSize: event.boardSize,
      state: GameState.WaitingForPlayers,
      result: GameResult.None,
      currentPlayer: null,
      createdAt: new Date(),
      startedAt: null,
      endedAt: null,
      moveCount: 0,
      isPrivate: event.isPrivate
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
          boardSize: event.boardSize,
          state: GameState.InProgress,
          startedAt: new Date()
        };
      }

      // No existing game (joiner). Create a fresh game instance.
      return {
        id: event.gameId,
        player1: event.player1,
        player2: event.player2,
        board: Array(event.boardSize).fill(null).map(() => Array(event.boardSize).fill('')),
        boardSize: event.boardSize,
        state: GameState.InProgress,
        result: GameResult.None,
        currentPlayer: event.currentPlayer,
        createdAt: new Date(), // We don't have the original timestamp; use "now" for reference
        startedAt: new Date(),
        endedAt: null,
        moveCount: 0,
        isPrivate: false // Default to false for joiners since we don't have this info in the event
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
    setError(`${event.playerName} scared of you and left the match`);
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
  const createGame = useCallback(async (playerName: string, characterIcon: CharacterIcon, boardSize: number = 3, isPrivate: boolean = false, playWithAI: boolean = false) => {
    try {
      await gameService.createGame(playerName, characterIcon, boardSize, isPrivate, playWithAI);
      setError(null);
    } catch (error) {
      setError(`Failed to create game: ${error}`);
    }
  }, []);

  const joinGame = useCallback(async (gameId: string, playerName: string, characterIcon: CharacterIcon) => {
    try {
      await gameService.joinGame(gameId, playerName, characterIcon);
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

  const leaveGame = useCallback(async () => {
    if (!currentGame) return;

    try {
      // Call the server's LeaveGame method to properly clean up the game state
      await gameService.leaveGame();
      
      // Clear the local game state
      setCurrentGame(null);
      setError(null);
    } catch (error) {
      setError(`Failed to leave game: ${error}`);
    }
  }, [currentGame]);

  const clearError = useCallback(() => {
    setError(null);
  }, []);

  // Game info helpers
  const isMyTurn = currentGame?.currentPlayer?.id === currentPlayerId;
  const canMakeMove = useCallback((row: number, col: number) => {
    return currentGame?.state === GameState.InProgress && 
           isMyTurn && 
           currentGame?.board[row][col] === '';
  }, [currentGame, isMyTurn]);

  const isGameInProgress = currentGame?.state === GameState.InProgress;
  const isGameFinished = currentGame?.state === GameState.Finished;
  const isWaitingForPlayer = currentGame?.state === GameState.WaitingForPlayers;

  const me = currentGame?.player1?.id === currentPlayerId ? currentGame?.player1 ?? null : 
             currentGame?.player2?.id === currentPlayerId ? currentGame?.player2 ?? null : null;

  const opponent = currentGame?.player1?.id !== currentPlayerId ? currentGame?.player1 ?? null : 
                   currentGame?.player2?.id !== currentPlayerId ? currentGame?.player2 ?? null : null;

  const myCharacterIcon = me?.characterIcon || null;

  return {
    // Game state
    currentGame,
    waitingGames,
    connectionState,
    error,
    isConnected,
    currentPlayerId,
    
    // Game actions
    connect,
    disconnect,
    createGame,
    joinGame,
    makeMove,
    getWaitingGames,
    resetGame,
    leaveGame,
    clearError,
    
    // Game info
    isMyTurn,
    canMakeMove,
    isGameInProgress,
    isGameFinished,
    isWaitingForPlayer,
    myCharacterIcon,
    opponent,
    me
  };
}; 