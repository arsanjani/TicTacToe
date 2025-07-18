import type { GameDto, CreateGameRequest, UpdateGameRequest } from '../types/gameDto';
import { GameState } from '../types/game';

export class GamesApiService {
  private readonly baseUrl = '/api/games';

  private async handleResponse<T>(response: Response): Promise<T> {
    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`HTTP ${response.status}: ${errorText}`);
    }
    
    const data = await response.json();
    
    // Convert date strings to Date objects
    if (data) {
      this.convertDates(data);
    }
    
    return data;
  }

  private convertDates(obj: unknown): void {
    if (Array.isArray(obj)) {
      obj.forEach(item => this.convertDates(item));
      return;
    }
    
    if (obj && typeof obj === 'object') {
      for (const key in obj) {
        if (Object.prototype.hasOwnProperty.call(obj, key)) {
          const value = (obj as Record<string, unknown>)[key];
          if (typeof value === 'string' && this.isDateString(key)) {
            (obj as Record<string, unknown>)[key] = new Date(value);
          } else if (typeof value === 'object') {
            this.convertDates(value);
          }
        }
      }
    }
  }

  private isDateString(key: string): boolean {
    const dateFields = ['createdAt', 'startedAt', 'endedAt', 'updatedAt'];
    return dateFields.includes(key);
  }

  /**
   * Get game by GameId (string identifier)
   */
  async getByGameId(gameId: string): Promise<GameDto> {
    const response = await fetch(`${this.baseUrl}/by-game-id/${encodeURIComponent(gameId)}`);
    return this.handleResponse<GameDto>(response);
  }

  /**
   * Get games by player ID
   */
  async getByPlayerId(playerId: string): Promise<GameDto[]> {
    const response = await fetch(`${this.baseUrl}/by-player/${encodeURIComponent(playerId)}`);
    return this.handleResponse<GameDto[]>(response);
  }

  /**
   * Get games by state
   */
  async getByState(state: GameState): Promise<GameDto[]> {
    const response = await fetch(`${this.baseUrl}/by-state/${encodeURIComponent(state)}`);
    return this.handleResponse<GameDto[]>(response);
  }

  /**
   * Get active games (waiting for players or in progress)
   */
  async getActiveGames(): Promise<GameDto[]> {
    const response = await fetch(`${this.baseUrl}/active`);
    return this.handleResponse<GameDto[]>(response);
  }

  /**
   * Get finished games
   */
  async getFinishedGames(): Promise<GameDto[]> {
    const response = await fetch(`${this.baseUrl}/finished`);
    return this.handleResponse<GameDto[]>(response);
  }

  /**
   * Create a new game
   */
  async createGame(request: CreateGameRequest): Promise<GameDto> {
    const response = await fetch(this.baseUrl, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(request),
    });
    return this.handleResponse<GameDto>(response);
  }

  /**
   * Update game by GameId
   */
  async updateByGameId(gameId: string, request: UpdateGameRequest): Promise<GameDto> {
    const response = await fetch(`${this.baseUrl}/by-game-id/${encodeURIComponent(gameId)}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(request),
    });
    return this.handleResponse<GameDto>(response);
  }

  /**
   * Update game by ID (inherited from BaseApiController)
   */
  async updateById(id: number, request: UpdateGameRequest): Promise<GameDto> {
    const response = await fetch(`${this.baseUrl}/${id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(request),
    });
    return this.handleResponse<GameDto>(response);
  }

  /**
   * Get game by ID (inherited from BaseApiController)
   */
  async getById(id: number): Promise<GameDto> {
    const response = await fetch(`${this.baseUrl}/${id}`);
    return this.handleResponse<GameDto>(response);
  }

  /**
   * Get all games (inherited from BaseApiController)
   */
  async getAll(): Promise<GameDto[]> {
    const response = await fetch(this.baseUrl);
    return this.handleResponse<GameDto[]>(response);
  }

  /**
   * Delete game by ID (inherited from BaseApiController)
   */
  async deleteById(id: number): Promise<void> {
    const response = await fetch(`${this.baseUrl}/${id}`, {
      method: 'DELETE',
    });
    
    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`HTTP ${response.status}: ${errorText}`);
    }
  }
}

// Export a singleton instance
export const gamesApiService = new GamesApiService(); 