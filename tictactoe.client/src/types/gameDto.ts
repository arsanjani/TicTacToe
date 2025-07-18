import { CharacterIcon, GameState, GameResult } from './game';

export interface BaseDto {
  id: number;
}

export interface GameDto extends BaseDto {
  gameId: string;
  player1Id: string;
  player1Name: string;
  player1Character: CharacterIcon;
  player2Id?: string;
  player2Name?: string;
  player2Character?: CharacterIcon;
  isPrivate: boolean;
  state: GameState;
  result: GameResult;
  winnerId?: string;
  winnerName?: string;
  createdAt: Date;
  startedAt?: Date;
  endedAt?: Date;
  totalMoves: number;
  duration?: number; // Duration in milliseconds (converted from TimeSpan)
}

export interface CreateGameRequest {
  player1Name: string;
  player1Character: CharacterIcon;
  isPrivate: boolean;
}

export interface UpdateGameRequest {
  player2Id?: string;
  player2Name?: string;
  player2Character?: CharacterIcon;
  state?: GameState;
  result?: GameResult;
  winnerId?: string;
  winnerName?: string;
  startedAt?: Date;
  endedAt?: Date;
  totalMoves?: number;
  duration?: number;
} 