export interface Player {
  id: string;
  name: string;
  symbol: 'X' | 'O';
  isReady: boolean;
  isActive: boolean;
  joinedAt: Date;
}

export interface Game {
  id: string;
  player1: Player | null;
  player2: Player | null;
  board: string[][];
  state: GameState;
  result: GameResult;
  currentPlayer: Player | null;
  createdAt: Date;
  startedAt: Date | null;
  endedAt: Date | null;
  moveCount: number;
  isPrivate: boolean;
}

export const GameState = {
  WaitingForPlayers: 'WaitingForPlayers',
  InProgress: 'InProgress',
  Finished: 'Finished'
} as const;

export type GameState = typeof GameState[keyof typeof GameState];

export const GameResult = {
  None: 'None',
  Player1Wins: 'Player1Wins',
  Player2Wins: 'Player2Wins',
  Draw: 'Draw'
} as const;

export type GameResult = typeof GameResult[keyof typeof GameResult];

export interface GameListItem {
  gameId: string;
  player1Name: string;
  createdAt: Date;
  isPrivate: boolean;
}

export interface GameCreatedEvent {
  gameId: string;
  playerId: string;
  playerName: string;
  symbol: 'X' | 'O';
  state: string;
  isPrivate: boolean;
}

export interface GameStartedEvent {
  gameId: string;
  player1: Player;
  player2: Player;
  currentPlayer: Player;
  state: string;
}

export interface MoveMadeEvent {
  gameId: string;
  row: number;
  col: number;
  playerId: string;
  board: string[][];
  currentPlayer: Player | null;
  moveCount: number;
}

export interface GameFinishedEvent {
  gameId: string;
  result: string;
  winner: string | null;
  endedAt: Date;
}

export interface GameResetEvent {
  gameId: string;
  board: string[][];
  currentPlayer: Player;
  state: string;
}

export interface PlayerDisconnectedEvent {
  gameId: string;
  playerId: string;
  playerName: string;
}

export interface GameError {
  message: string;
} 