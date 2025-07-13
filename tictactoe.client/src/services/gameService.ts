import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import type { 
  GameCreatedEvent, 
  GameStartedEvent, 
  MoveMadeEvent, 
  GameFinishedEvent, 
  GameResetEvent, 
  PlayerDisconnectedEvent, 
  GameError, 
  GameListItem 
} from '../types/game';

export class GameService {
  private connection: HubConnection;
  private isConnected = false;

  constructor() {
    this.connection = new HubConnectionBuilder()
      .withUrl('/gameHub')
      .build();

    this.setupEventHandlers();
  }

  private setupEventHandlers(): void {
    this.connection.on('GameCreated', (event: GameCreatedEvent) => {
      this.onGameCreated?.(event);
    });

    this.connection.on('GameStarted', (event: GameStartedEvent) => {
      this.onGameStarted?.(event);
    });

    this.connection.on('MoveMade', (event: MoveMadeEvent) => {
      this.onMoveMade?.(event);
    });

    this.connection.on('GameFinished', (event: GameFinishedEvent) => {
      this.onGameFinished?.(event);
    });

    this.connection.on('GameReset', (event: GameResetEvent) => {
      this.onGameReset?.(event);
    });

    this.connection.on('PlayerDisconnected', (event: PlayerDisconnectedEvent) => {
      this.onPlayerDisconnected?.(event);
    });

    this.connection.on('WaitingGames', (games: GameListItem[]) => {
      this.onWaitingGames?.(games);
    });

    this.connection.on('Error', (error: string) => {
      this.onError?.({ message: error });
    });
  }

  // Event handlers (to be set by components)
  public onGameCreated?: (event: GameCreatedEvent) => void;
  public onGameStarted?: (event: GameStartedEvent) => void;
  public onMoveMade?: (event: MoveMadeEvent) => void;
  public onGameFinished?: (event: GameFinishedEvent) => void;
  public onGameReset?: (event: GameResetEvent) => void;
  public onPlayerDisconnected?: (event: PlayerDisconnectedEvent) => void;
  public onWaitingGames?: (games: GameListItem[]) => void;
  public onError?: (error: GameError) => void;
  public onConnectionEstablished?: (connectionId: string) => void;

  async connect(): Promise<void> {
    if (this.isConnected) return;

    try {
      await this.connection.start();
      this.isConnected = true;
      console.log('Connected to game hub');
      this.onConnectionEstablished?.(this.connection.connectionId || '');
    } catch (error) {
      console.error('Failed to connect to game hub:', error);
      throw error;
    }
  }

  async disconnect(): Promise<void> {
    if (!this.isConnected) return;

    try {
      await this.connection.stop();
      this.isConnected = false;
      console.log('Disconnected from game hub');
    } catch (error) {
      console.error('Failed to disconnect from game hub:', error);
      throw error;
    }
  }

  async createGame(playerName: string): Promise<void> {
    if (!this.isConnected) {
      throw new Error('Not connected to game hub');
    }

    try {
      await this.connection.invoke('CreateGame', playerName);
    } catch (error) {
      console.error('Failed to create game:', error);
      throw error;
    }
  }

  async joinGame(gameId: string, playerName: string): Promise<void> {
    if (!this.isConnected) {
      throw new Error('Not connected to game hub');
    }

    try {
      await this.connection.invoke('JoinGame', gameId, playerName);
    } catch (error) {
      console.error('Failed to join game:', error);
      throw error;
    }
  }

  async makeMove(gameId: string, row: number, col: number): Promise<void> {
    if (!this.isConnected) {
      throw new Error('Not connected to game hub');
    }

    try {
      await this.connection.invoke('MakeMove', gameId, row, col);
    } catch (error) {
      console.error('Failed to make move:', error);
      throw error;
    }
  }

  async getWaitingGames(): Promise<void> {
    if (!this.isConnected) {
      throw new Error('Not connected to game hub');
    }

    try {
      await this.connection.invoke('GetWaitingGames');
    } catch (error) {
      console.error('Failed to get waiting games:', error);
      throw error;
    }
  }

  async resetGame(gameId: string): Promise<void> {
    if (!this.isConnected) {
      throw new Error('Not connected to game hub');
    }

    try {
      await this.connection.invoke('ResetGame', gameId);
    } catch (error) {
      console.error('Failed to reset game:', error);
      throw error;
    }
  }

  getConnectionState(): string {
    return this.connection.state;
  }
}

// Create a singleton instance
export const gameService = new GameService(); 