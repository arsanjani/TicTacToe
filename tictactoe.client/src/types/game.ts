export const CharacterIcon = {
  Cross: 'Cross',
  Circle: 'Circle',
  Kuromi: 'Kuromi',
  MyMelody: 'MyMelody',
  Spiderman: 'Spiderman',
  Cinnamoroll: 'Cinnamoroll',
  BadtzMaru: 'BadtzMaru',
  HelloKitty: 'HelloKitty',
  Keroppi: 'Keroppi',
  Pochacco: 'Pochacco',
  AI: 'AI'
} as const;

export type CharacterIcon = typeof CharacterIcon[keyof typeof CharacterIcon];

export interface Player {
  id: string;
  name: string;
  characterIcon: CharacterIcon;
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
  player1CharacterIcon: CharacterIcon;
  createdAt: Date;
  isPrivate: boolean;
}

export interface GameCreatedEvent {
  gameId: string;
  playerId: string;
  playerName: string;
  characterIcon: CharacterIcon;
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

export interface CharacterIconInfo {
  icon: CharacterIcon;
  displayName: string;
  fileName: string;
}

export const CHARACTER_ICONS: CharacterIconInfo[] = [
  { icon: CharacterIcon.Cross, displayName: 'Cross', fileName: 'icons8-cross-100.png' },
  { icon: CharacterIcon.Circle, displayName: 'Circle', fileName: 'icons8-circle-100-4.png' },
  { icon: CharacterIcon.Kuromi, displayName: 'Kuromi', fileName: 'icons8-kuromi-100.png' },
  { icon: CharacterIcon.MyMelody, displayName: 'My Melody', fileName: 'icons8-my-melody-100.png' },
  { icon: CharacterIcon.Spiderman, displayName: 'Spider-Man', fileName: 'icons8-spiderman-100-2.png' },
  { icon: CharacterIcon.Cinnamoroll, displayName: 'Cinnamoroll', fileName: 'icons8-cinnamoroll-100.png' },
  { icon: CharacterIcon.BadtzMaru, displayName: 'Badtz-Maru', fileName: 'icons8-badtz-maru-100.png' },
  { icon: CharacterIcon.HelloKitty, displayName: 'Hello Kitty', fileName: 'icons8-hello-kitty-100.png' },
  { icon: CharacterIcon.Keroppi, displayName: 'Keroppi', fileName: 'icons8-keroppi-100.png' },
  { icon: CharacterIcon.Pochacco, displayName: 'Pochacco', fileName: 'icons8-pochacco-100.png' },
  { icon: CharacterIcon.AI, displayName: 'AI', fileName: 'icons8-ai-100.png' }
];

// Re-export GameDto types for convenience
export type { GameDto, BaseDto, CreateGameRequest, UpdateGameRequest } from './gameDto'; 