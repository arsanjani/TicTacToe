import type { GameDto, CreateGameRequest, UpdateGameRequest } from '../types/gameDto';
import { GameState } from '../types/game';
import { BaseApiService } from "./baseApiService";

export class GamesApiService extends BaseApiService {
  protected readonly baseUrl = "/api/games";

  /**
   * Get game by GameId (string identifier)
   */
  async getByGameId(gameId: string): Promise<GameDto> {
    return this.get<GameDto>(
      `${this.baseUrl}/by-game-id/${encodeURIComponent(gameId)}`
    );
  }

  /**
   * Get games by player ID
   */
  async getByPlayerId(playerId: string): Promise<GameDto[]> {
    return this.get<GameDto[]>(
      `${this.baseUrl}/by-player/${encodeURIComponent(playerId)}`
    );
  }

  /**
   * Get games by state
   */
  async getByState(state: GameState): Promise<GameDto[]> {
    return this.get<GameDto[]>(
      `${this.baseUrl}/by-state/${encodeURIComponent(state)}`
    );
  }

  /**
   * Get active games (waiting for players or in progress)
   */
  async getActiveGames(): Promise<GameDto[]> {
    return this.get<GameDto[]>(`${this.baseUrl}/active`);
  }

  /**
   * Get finished games
   */
  async getFinishedGames(): Promise<GameDto[]> {
    return this.get<GameDto[]>(`${this.baseUrl}/finished`);
  }

  /**
   * Create a new game
   */
  async createGame(request: CreateGameRequest): Promise<GameDto> {
    return this.post<GameDto>(this.baseUrl, request);
  }

  /**
   * Update game by GameId
   */
  async updateByGameId(
    gameId: string,
    request: UpdateGameRequest
  ): Promise<GameDto> {
    return this.put<GameDto>(
      `${this.baseUrl}/by-game-id/${encodeURIComponent(gameId)}`,
      request
    );
  }
}

// Export a singleton instance
export const gamesApiService = new GamesApiService(); 