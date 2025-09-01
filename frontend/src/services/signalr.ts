import * as signalR from '@microsoft/signalr';
import { CryptoData } from '../types/crypto';

class SignalRService {
  private connection: signalR.HubConnection | null = null;
  private connectionPromise: Promise<void> | null = null;

  async connect(): Promise<void> {
    if (this.connection?.state === signalR.HubConnectionState.Connected) {
      return;
    }

    if (this.connectionPromise) {
      return this.connectionPromise;
    }

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('https://crypto-dashboard-api-m6lg.onrender.com/hubs/crypto', {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();

    this.connectionPromise = this.connection.start();
    
    try {
      await this.connectionPromise;
      console.log('SignalR Connected!');
    } catch (err) {
      console.error('SignalR Connection Error:', err);
      this.connectionPromise = null;
      throw err;
    }
  }

  async subscribe(symbol: string): Promise<void> {
    if (!this.connection) await this.connect();
    await this.connection!.invoke('Subscribe', symbol);
  }

  async unsubscribe(symbol: string): Promise<void> {
    if (!this.connection) return;
    await this.connection.invoke('Unsubscribe', symbol);
  }

  onPriceUpdate(callback: (data: CryptoData) => void): void {
    if (!this.connection) return;
    this.connection.on('PriceUpdate', callback);
  }

  onMarketUpdate(callback: (data: CryptoData[]) => void): void {
    if (!this.connection) return;
    this.connection.on('MarketUpdate', callback);
  }

  async disconnect(): Promise<void> {
    if (this.connection) {
      await this.connection.stop();
      this.connection = null;
      this.connectionPromise = null;
    }
  }
}

export default new SignalRService();
